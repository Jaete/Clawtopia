using Godot;
using System;


public partial class State : Node
{
    [Signal]
    public delegate void StateTransitionEventHandler(State current, String next);
    
    // VARIAVEIS DE REFERENCIA
    public Ally self;
    public SimulationMode simulation_mode;
    public BuildMode build_mode;
    public ModeManager mode_manager;
    public Controller controller;
    public override void _Ready() {
        Initialize();
    }
    public virtual void Enter(){}
    public virtual void Update(double _delta){}
    public virtual void Exit(){}
    public virtual void When_mouse_right_clicked(Vector2 coords){}
    public virtual void When_navigation_finished(){}

    /// <summary>
    /// Inicializacao basica para todos os estados.
    /// Cada estado chama esta funcao em sua própria <c>_Ready()</c> e após isso inicializa dados especificos
    /// daquele estado. <br></br><br></br>&#10;
    /// No caso de nao haver dado especifico, como no estado <c>Idle</c>, a _Ready do estado base
    /// já realiza este trabalho.
    /// </summary>
    public void Initialize(){
        self = GetParent().GetParent<Ally>();
        controller = GetNode<Controller>("/root/Game/Controller");
        mode_manager = GetNode<ModeManager>("/root/Game/ModeManager");
        simulation_mode = mode_manager.GetNode<SimulationMode>("SimulationMode");
        build_mode = mode_manager.GetNode<BuildMode>("BuildMode");
        self.agent.VelocityComputed += When_velocity_computed;
        build_mode.ConstructionStarted += When_build_started;
        build_mode.BuildCompleted += When_build_completed;
        if (self.category == Constants.MILITARY){
            
        }
    }

    public void Change_state(string next){
        EmitSignal("StateTransition", this, next);
    }
    
    public void Set_target_position(Vector2 coords){
        self.agent.TargetPosition = coords;
    }
    public void When_build_started(Building building){
        if (!self.currently_selected){ return; }
        self.ally_is_building = true;
        self.construction_to_build = building;
        Set_target_position(self.construction_to_build.GlobalPosition);
        Change_state("Move");
    }
    public void When_build_completed(Building building){
        if (build_mode.current_constructors.Contains(self)){
            self.ally_is_building = false;
            Change_state("Idle");
        }
    }
    public void When_velocity_computed(Vector2 safe_velocity) {
        self.Velocity = safe_velocity * self.attributes.move_speed;
        self.MoveAndSlide();
    }
    public void Choose_next_target_position_ECONOMIC(Vector2 coords){
        self.interacted_with_building = simulation_mode.building_to_interact != null;
        if (self.interacted_with_building){
            Set_target_position(recalculate_coords(
                self.GlobalPosition,
                simulation_mode.building_to_interact.GlobalPosition));
            self.interacted_building = simulation_mode.building_to_interact;
        }else if (Is_interacting_with_water_at(coords)){
            self.interacted_resource = Constants.SALMON; // VARIAVEL NO ALIADO BASE PARA REFERENCIA
            self.interacted_building = null;
            self.current_resource_last_position = Get_closest_water_coord();
            Set_target_position(self.current_resource_last_position);
        }else{
            self.interacted_resource = null;
            self.interacted_building = null;
            Set_target_position(coords);
        }
    }

    public void Choose_next_target_position_MILITARY(Vector2 coords){
        Set_target_position(coords);
    }
    
    /// <summary>
    /// Recalcula as coordenadas baseado na posicao do aliado e da construçao selecionada.
    /// Pressupoe-se que a este ponto o clique direito foi efetuado em cima de uma construcao.
    /// </summary>
    /// <param name="ally_position">A posicao global do aliado.</param>
    /// <param name="building_position">A posicao global da construcao.</param>
    /// <returns> <c>Vector2</c> coordenadas globais.</returns>
    public Vector2 recalculate_coords(Vector2 ally_position, Vector2 building_position){
        BuildMode build_mode = mode_manager.GetNode<BuildMode>("BuildMode");
        var x = building_position.X - ally_position.X > 0? -1 : 1;
        var y = building_position.Y - ally_position.Y > 0? 1 : -1;
        var new_coords = new Vector2(
            building_position.X + (build_mode.TILE_SIZE_X * x),
            building_position.Y + (build_mode.TILE_SIZE_Y * y)
        );
        return new_coords;
    }
    /// <summary>
    /// Verifica se esta interagindo com um tile de agua, usando DataLayer no Tileset.
    /// Pressupoe-se que nao esta interagindo com uma estrutura ou inimigo, pois a verificacao
    /// deles é feita primeiro.
    /// </summary>
    /// <param name="coords">A posicao global do clique.</param>
    /// <returns> <c>bool</c> Se é agua ou não.</returns>
    public bool Is_interacting_with_water_at(Vector2 coords){
        var map_coords = simulation_mode.tile_map.LocalToMap(coords);
        var data = simulation_mode.tile_map.GetCellTileData(0, map_coords, true);
        return (bool) data.GetCustomData("is_water");
    }
    
    /// <summary>
    /// Procura todos os tiles de agua no cenario e retorna as coordenadas globais do
    /// tile mais proximo.
    /// </summary>
    /// <returns> <c>Vector2</c> Coordenadas globais do tile mais proximo.</returns>
    public Vector2 Get_closest_water_coord(){
        var tilemap = simulation_mode.tile_map;
        var water_tiles = tilemap.GetUsedCellsById(1);
        Vector2 closest_tile = default;
        foreach (var tile in water_tiles){
            var local_tile_pos = tilemap.MapToLocal(tile);
            var global_tile_pos = tilemap.ToGlobal(local_tile_pos);
            if (closest_tile == default){
                closest_tile = global_tile_pos;
            }else if (self.GlobalPosition.DistanceSquaredTo(closest_tile) > self.GlobalPosition.DistanceSquaredTo(global_tile_pos)){
                closest_tile = global_tile_pos;
            }
        }
        return closest_tile;
    }

    /// <summary>
    /// Procura todos os nodes de construcoes do recurso especifico no cenario
    /// e retorna as coordenadas globais da construcao mais proxima.
    ///
    /// Utiliza-se da funcao <c>Compare_distance_to_building</c> para evitar repeticao
    /// de codigo.
    /// </summary>
    /// <returns> <c>Vector2</c> Coordenadas globais da construcao mais proxima</returns>
    public Vector2 Get_closest_resource_building(Vector2 coords, string resource){
        Vector2 closest_building_position = default;
        switch (resource){
            case Constants.SALMON:
                foreach (var building in self.level_manager.salmon_buildings){
                    closest_building_position = Compare_distance_to_building(coords, closest_building_position, building);
                }
                break;
            case Constants.CATNIP:
                foreach (var building in self.level_manager.catnip_buildings){ 
                    closest_building_position = Compare_distance_to_building(coords, closest_building_position, building);
                }
                break;
            case Constants.SAND:
                foreach (var building in self.level_manager.sand_buildings){ 
                    closest_building_position = Compare_distance_to_building(coords, closest_building_position, building);
                }
                break;
        }
        return closest_building_position;
    }

    public Vector2 Compare_distance_to_building(Vector2 coords, Vector2 closest_building_position, Building building){
        // SE É A PRIMEIRA ITERACAO, ARMAZENA DIRETO
        if (closest_building_position == default){
            return building.GlobalPosition;
        }
        // SENAO, COMPARA DISTANCIA
        if (closest_building_position.DistanceSquaredTo(coords) > building.GlobalPosition.DistanceSquaredTo(coords)){
            return building.GlobalPosition;
        }
        return closest_building_position;
    }

    public void Move(){
        var next_path_pos = self.agent.GetNextPathPosition();
        var new_velocity = self.GlobalPosition.DirectionTo(next_path_pos);
        if (self.agent.AvoidanceEnabled){
            self.agent.Velocity = new_velocity;
        }
        else{
            When_velocity_computed(new_velocity);
        }
    }
}
