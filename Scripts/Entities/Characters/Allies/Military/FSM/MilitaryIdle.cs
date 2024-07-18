using Godot;
using System;

public partial class MilitaryIdle : AllyState
{
    public override void _Ready(){
        ally = GetParent().GetParent<Ally>();
        controller.MouseRightPressed += Start_move;
    }

    public override void Enter(){
        ally.Velocity = Vector2.Zero;
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
}
