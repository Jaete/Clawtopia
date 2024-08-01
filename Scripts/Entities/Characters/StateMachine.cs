using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public partial class StateMachine : Node
{
    public NavigationAgent2D navigation;
    public Controller controller;
    
    public Dictionary states = new();
    public State current_state;
    public bool in_transition;
    
    [Export] 
    public State default_state;

    public override void _Ready(){
        navigation = GetNode<NavigationAgent2D>("../Navigation");
        controller = GetNode<Controller>("/root/Game/Controller");
        Array<Node> node_states = GetChildren();
        foreach(var node in node_states){
            if (node is State state){
                state.StateTransition += Change_state;
                states[state.Name] = state;
            }
        }
        current_state = (State)states[default_state.Name];
        current_state.Enter();
        controller.MouseRightPressed += When_mouse_right_clicked;
        navigation.NavigationFinished += When_navigation_finished;
    }

    public override void _PhysicsProcess(double delta){
        if(!in_transition){
            current_state.Update(delta);
        }
    }

    public virtual void Change_state(State current, String next){
        in_transition = true;
        if(current.Name == next) {
            in_transition = false;
            return;
        }
        current_state.Exit();
        current_state = (State)states[next]; ;
        current_state.Enter();
        in_transition = false;
    }
    
    public void When_mouse_right_clicked(Vector2 coords){
        current_state.When_mouse_right_clicked(coords);
    }

    public void When_navigation_finished(){
        current_state.When_navigation_finished();
    }
}

