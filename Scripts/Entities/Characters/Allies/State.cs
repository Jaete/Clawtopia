using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using static Godot.WebSocketPeer;

public partial class AllyState : Node
{
    public const string CATNIP = "catnip";
    public const string SALMON = "salmon";
    public const string SAND  = "sand";

    [Signal]
    public delegate void StateTransitionEventHandler(AllyState current, String next);

    public Ally ally;
    
    public SimulationMode simulation_mode;
    public ModeManager mode_manager;
    
    public Controller controller;
    
    public bool interacted_with_building;
    
    
    
    public override void _Ready() {
        Initialize();
    }
    
    public virtual void Enter(){
    }

    public virtual void Update(double _delta) {
    }

    public virtual void Exit(){
    }

    /// <summary>
    /// Inicializacao basica para todos os estados.
    /// Cada estado chama esta funcao em sua própria <c>_Ready()</c> e após isso inicializa dados especificos
    /// daquele estado. <br></br><br></br>&#10;
    /// No caso de nao haver dado especifico, como no estado <c>Idle</c>, a _Ready do estado base
    /// já realiza este trabalho.
    /// </summary>
    public void Initialize(){
        ally = GetParent().GetParent<Ally>();
        controller = GetNode<Controller>("/root/Game/Controller");
        mode_manager = GetNode<ModeManager>("/root/Game/ModeManager");
        simulation_mode = mode_manager.GetNode<SimulationMode>("SimulationMode");
        ally.agent.VelocityComputed += When_velocity_computed;
        controller.MouseRightPressed += When_mouse_right_clicked;
        ally.agent.NavigationFinished += When_navigation_finished;
    }

    public void Change_state(string next){
        EmitSignal("StateTransition", this, next);
    }

    public void Set_target_position(Vector2 coords){
        ally.agent.TargetPosition = coords;
        
    }

    public void When_velocity_computed(Vector2 safe_velocity) {
        ally.Velocity = safe_velocity * ally.attributes.move_speed;
        ally.MoveAndSlide();
    }

    public virtual void When_mouse_right_clicked(Vector2 coords){
    }

    public virtual void When_navigation_finished(){
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
            building_position.X + (build_mode.TILE_SIZE_X * x) * 2,
            building_position.Y + (build_mode.TILE_SIZE_Y * y) * 2
        );
        return new_coords;
    }

    public bool Is_interacting_with_water_at(Vector2 coords){
        var map_coords = simulation_mode.tile_map.LocalToMap(coords);
        var data = simulation_mode.tile_map.GetCellTileData(0, map_coords, true);
        return (bool) data.GetCustomData("is_water");
    }

    public Vector2 Get_closest_water_coord(){
        var tilemap = simulation_mode.tile_map;
        var water_tiles = tilemap.GetUsedCellsById(1);
        Vector2 closest_tile = default;
        foreach (var tile in water_tiles){
            var local_tile_pos = tilemap.MapToLocal(tile);
            var global_tile_pos = tilemap.ToGlobal(local_tile_pos);
            if (closest_tile == default){
                closest_tile = global_tile_pos;
            }else if (ally.GlobalPosition.DistanceSquaredTo(closest_tile) > ally.GlobalPosition.DistanceSquaredTo(global_tile_pos)){
                closest_tile = global_tile_pos;
            }
        }
        return closest_tile;
    }

    public void Choose_next_target_position(Vector2 coords){
        if (!ally.currently_selected) return;
        interacted_with_building = simulation_mode.building_to_interact != null;
        if (interacted_with_building){
            Set_target_position(recalculate_coords(
                ally.GlobalPosition,
                simulation_mode.building_to_interact.GlobalPosition));
            ally.interacted_building = simulation_mode.building_to_interact;
        }else if (Is_interacting_with_water_at(coords)){
            ally.interacted_resource = "salmon"; // VARIAVEL NO ALIADO BASE PARA REFERENCIA
            ally.interacted_building = null;
            ally.current_resource_last_position = Get_closest_water_coord();
            Set_target_position(ally.current_resource_last_position);
        }else{
            ally.interacted_resource = null;
            ally.interacted_building = null;
            Set_target_position(coords);
        }
        Change_state("Move");
    }

    public Vector2 Get_closest_resource_building(Vector2 coords, string resource){
        /* Logica: 
         * Pega a coordenada do aliado e procura em todos os nodes de construcao daquele recurso
         * Esses nodes ficam no level manager na hora do load da fase
         * Calcula o distanceTo e retorna a coordenada do node mais proximo
         */
        GD.Print("SALMON BUILDINGS: ", ally.level_manager.salmon_buildings);
        Vector2 closest_building_position = default;
        switch (resource){
            case SALMON:
                foreach (var building in ally.level_manager.salmon_buildings){
                    closest_building_position = Compare_distance_to_building(coords, closest_building_position, building);
                }
                break;
            case CATNIP:
                foreach (var building in ally.level_manager.catnip_buildings){ 
                    closest_building_position = Compare_distance_to_building(coords, closest_building_position, building);
                }
                break;
            case SAND:
                foreach (var building in ally.level_manager.sand_buildings){ 
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

}
