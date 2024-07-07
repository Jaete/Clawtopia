using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class AllyState : Node
{

    [Signal]
    public delegate void StateTransitionEventHandler(AllyState current, String next);

    public Ally self;

    public virtual void Enter(){

    }

    public virtual void Update(){
        
    }

    public virtual void Exit(){
        
    }

    public void Change_state(String next){
        EmitSignal("StateTransition", this, next);
    }

    public void Set_target_position(Vector2 coords){
        self.agent.TargetPosition = coords;
    }
}
