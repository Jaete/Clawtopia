using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using static Godot.WebSocketPeer;

public partial class AllyState : Node
{

    [Signal]
    public delegate void StateTransitionEventHandler(AllyState current, String next);
    [Signal]
    public delegate void MouseRightClickedEventHandler(Vector2 coords);

    public Ally self;
    public override void _Ready() {
        self = GetParent().GetParent<Ally>();
        self.agent.VelocityComputed += On_velocity_computed;
    }
    public virtual void Enter(){

    }

    public virtual void Update(double _delta) {
    }

    public virtual void Exit(){
        
    }

    public void Change_state(String next){
        EmitSignal("StateTransition", this, next);
    }

    public void Set_target_position(Vector2 coords){
        self.agent.TargetPosition = coords;
    }

    public virtual void On_velocity_computed(Vector2 safe_velocity) {
        self.Velocity = safe_velocity;
        self.MoveAndSlide();
    }
}
