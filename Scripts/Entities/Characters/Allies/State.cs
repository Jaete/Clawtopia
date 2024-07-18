using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using static Godot.WebSocketPeer;

public partial class AllyState : Node
{

    [Signal]
    public delegate void StateTransitionEventHandler(AllyState current, String next);

    public Ally ally;
    
    public SimulationMode simulation_mode;
    public ModeManager mode_manager;
    
    public Controller controller;
    
    public bool interacted_with_building;
    public bool is_current_state;
    
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
            building_position.X + build_mode.TILE_SIZE_X * x,
            building_position.Y + build_mode.TILE_SIZE_Y * y
        );
        return new_coords;
    }
}
