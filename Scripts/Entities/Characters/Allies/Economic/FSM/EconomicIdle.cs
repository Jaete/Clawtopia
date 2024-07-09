using Godot;
using System;

public partial class EconomicIdle : AllyState {

    Director director;

    public override void _Ready(){
        self = GetParent().GetParent<Ally>();
        MouseRightClicked += When_move_started;
    }

    public override void Enter(){
        self.Velocity = Vector2.Zero;
       /* TODO
        Tocar animacao de idle quando houver
       */
    }

    public override void Update(double _delta){
        /*Nao faz nada, afinal e estado Idle*/
        GD.Print("I'm idle"); // <- Apenas para DEBUG, sera removido depois
    }

    public override void Exit(){
    }

    public void When_move_started(Vector2 coords){
        Change_state("Move");
        Set_target_position(coords);
    }

    public override void _Input(InputEvent @event){
        if(@event is InputEventMouseButton mouseEvent){
            if(mouseEvent.ButtonMask == MouseButtonMask.Right){
                EmitSignal("MouseRightClicked", mouseEvent.GlobalPosition);
            }
        }
    }
}
