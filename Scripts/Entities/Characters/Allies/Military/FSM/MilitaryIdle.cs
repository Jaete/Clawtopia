using Godot;
using System;

public partial class MilitaryIdle : AllyState
{
    [Signal]
    public delegate void MouseRightClickedEventHandler(Vector2 coords);
    Director director;

    public override void _Ready(){
        self = GetParent().GetParent<Ally>();
        MouseRightClicked += Start_move;
    }

    public override void Enter(){
        self.Velocity = Vector2.Zero;
    }

    public override void Update(double _delta){
        GD.Print("I'm idle.");
    }

    public override void Exit(){
        base.Exit();
    }

    public void Start_move(Vector2 coords){
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
