using Godot;
using System;

public partial class MilitaryMove : State
{
    public override void Enter(){
    }

    public override void Update(double _delta){
        GD.Print("I'm moving");
    }

    public override void Exit(){
    }
}
