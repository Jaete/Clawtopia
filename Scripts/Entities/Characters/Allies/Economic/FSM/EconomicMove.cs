using Godot;
using System;

public partial class EconomicMove : AllyState
{
    public override void Enter(){
    }

    public override void Update(double _delta){
        GD.Print("I'm moving");
        /*if (self.agent.IsNavigationFinished()) {
            Change_state("Idle");
            return;
        }
        float movement_delta = self.attributes.move_speed * (float)_delta;
        Vector2 next_path_pos = self.agent.GetNextPathPosition();
        Vector2 new_velocity = self.GlobalPosition.DirectionTo(next_path_pos) * ;*/
    }

    public override void Exit(){
        base.Exit();
    }
}
