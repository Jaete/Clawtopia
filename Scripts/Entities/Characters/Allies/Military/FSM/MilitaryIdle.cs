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
    
    public override void When_mouse_right_clicked(Vector2 coords){
        if (!self.currently_selected){ return; }
        Choose_next_target_position_MILITARY(coords);
        Change_state("Move");
    }
}
