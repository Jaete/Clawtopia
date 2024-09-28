using Godot;
using System;
using Godot.Collections;

public partial class StateMachine : Node
{
    public NavigationAgent2D Navigation;
    public Controller Controller;
    
    public Dictionary States = new();
    public State CurrentState;
    public bool InTransition;
    
    [Export] 
    public State DefaultState;
    
    public override void _Ready(){
        Navigation = GetNode<NavigationAgent2D>("../Navigation");
        Controller = GetNode<Controller>("/root/Game/Controller");
        Array<Node> nodeStates = GetChildren();
        foreach(var node in nodeStates){
            if (node is State state){
                state.StateTransition += Change_state;
                States[state.Name] = state;
            }
            
        }
        CurrentState = (State) States[DefaultState.Name];
        CurrentState.Enter();
        Controller.MouseRightPressed += MouseRightClicked;
        Navigation.NavigationFinished += NavigationFinished;
    }

    public override void _PhysicsProcess(double delta){
        if(!InTransition){
            CurrentState.Update(delta);
        }
    }

    public virtual void Change_state(State current, String next){
        InTransition = true;
        if(current.Name == next) {
            InTransition = false;
            return;
        }
        CurrentState.Exit();
        CurrentState = (State)States[next]; ;
        CurrentState.Enter();
        InTransition = false;
    }
    
    public void MouseRightClicked(Vector2 coords){
        CurrentState.MouseRightClicked(coords);
    }

    public void NavigationFinished(){
        CurrentState.NavigationFinished();
    }
}

