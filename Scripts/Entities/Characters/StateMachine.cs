using Godot;
using System;
using Godot.Collections;
using ClawtopiaCs.Scripts.Systems.GameModes;

public partial class StateMachine : Node
{
    public NavigationAgent2D Navigation;
    
    public Dictionary States = new();
    public State CurrentState;
    public bool InTransition;
    
    [Export] 
    public State DefaultState;
    
    public override void _Ready(){
        Navigation = GetNode<NavigationAgent2D>("../Navigation");
        Array<Node> nodeStates = GetChildren();
        foreach(var node in nodeStates){
            if (node is not State state){
                continue;
            }
            if (!state.IsConnected(State.SignalName.StateTransition, new Callable(this, MethodName.ChangeState))){
                state.StateTransition += ChangeState;
            }
            States[state.Name] = state;
        }
        CurrentState = (State)States[DefaultState.Name];
        CurrentState.Enter();

        SimulationMode.Singleton.AllyCommand += CommandReceived;
        Navigation.NavigationFinished += NavigationFinished;
    }

    public override void _PhysicsProcess(double delta){
        if(!InTransition){
            CurrentState.Update(delta);
        }
    }

    public virtual void ChangeState(State current, String next){
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
    
    public void CommandReceived(Vector2 coords){
        CurrentState.CommandReceived(coords);
    }

    public void NavigationFinished(){
        CurrentState.NavigationFinished();
    }
}

