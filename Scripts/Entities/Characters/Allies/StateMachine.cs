using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public partial class StateMachine : Node
{
    public Dictionary states = new();
    public AllyState current_state;
    public bool in_transition;
    
    [Export] 
    public AllyState default_state;

    public override void _Ready(){
        Array<Node> node_states = GetChildren();
        foreach(var node in node_states){
            if (node is AllyState state){
                state.StateTransition += Change_state;
                states[state.Name] = state;
            }
        }
        current_state = (AllyState)states[default_state.Name];
        current_state.Enter();
    }

    public override void _PhysicsProcess(double delta){
        if(!in_transition){
            current_state.Update(delta);
        }
    }

    public virtual void Change_state(AllyState current, String next){
        in_transition = true;
        if(current.Name == next) {
            in_transition = false;
            return;
        }
        current_state.Exit();
        current_state = (AllyState)states[next]; ;
        current_state.Enter();
        in_transition = false;
    }
}

