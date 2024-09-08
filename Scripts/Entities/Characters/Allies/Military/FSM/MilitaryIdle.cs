using Godot;
using System;

public partial class MilitaryIdle : State
{
    public override void Enter(){
        self.Velocity = Vector2.Zero;
    }

    public override void Update(double _delta){
    }

    public override void Exit(){
    }
}
