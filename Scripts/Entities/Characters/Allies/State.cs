using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using static Godot.WebSocketPeer;

public partial class AllyState : Node
{

    [Signal]
    public delegate void StateTransitionEventHandler(AllyState current, String next);

    public Ally self;
    public SimulationMode simulation_mode;
    public Controller controller;
    public bool interacted_with_building;
    
    public override void _Ready() {
        self = GetParent().GetParent<Ally>();
        controller = GetNode<Controller>("/root/Game/Controller");
        simulation_mode = GetNode<SimulationMode>("/root/Game/ModeManager/SimulationMode");
        self.agent.VelocityComputed += On_velocity_computed;
        controller.MouseRightPressed += When_mouse_right_clicked;
    }
    public virtual void Enter(){

    }

    public virtual void Update(double _delta) {
    }

    public virtual void Exit(){
        
    }

    public void Change_state(string next){
        EmitSignal("StateTransition", this, next);
    }

    public void Set_target_position(Vector2 coords){
        self.agent.TargetPosition = coords;
    }

    public void On_velocity_computed(Vector2 safe_velocity) {
        self.Velocity = safe_velocity;
        self.MoveAndSlide();
    }

    public virtual void When_mouse_right_clicked(Vector2 coords){
        
    }
}
