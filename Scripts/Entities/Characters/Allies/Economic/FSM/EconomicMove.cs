using Godot;
using System;

public partial class EconomicMove : AllyState
{
    public override void Enter(){
    }

    public override void Update(double _delta){
        if (self.agent.IsNavigationFinished()) {
            if (interacted_with_building){
                switch (simulation_mode.building_to_interact.data.TYPE){
                    case "Tower":
                        break;
                    case "Resource":
                        break;
                    case "GreatCommune":
                        break;
                }
                Change_state("InteractedWithBuilding");
            }
            Change_state("Idle");
            return;
        }
        var next_path_pos = self.agent.GetNextPathPosition();
        var new_velocity = self.GlobalPosition.DirectionTo(next_path_pos) * self.attributes.move_speed;
        if (self.agent.AvoidanceEnabled){
            self.agent.Velocity = new_velocity;
        }else{
            On_velocity_computed(new_velocity);
        }
    }

    public override void Exit(){
        base.Exit();
    }
}
