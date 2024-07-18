using Godot;
using Godot.Collections;

public partial class EconomicMove : AllyState
{
    public override void Enter(){
    }

    public override void Update(double _delta){
        var next_path_pos = ally.agent.GetNextPathPosition();
        var new_velocity = ally.GlobalPosition.DirectionTo(next_path_pos);
        if (ally.agent.AvoidanceEnabled){
            ally.agent.Velocity = new_velocity.Normalized();
        }else{
            When_velocity_computed(new_velocity.Normalized());
        }
        if (Input.IsActionPressed("RightClick") && ally.currently_selected){
            Set_target_position(ally.GetGlobalMousePosition());
        }
        Check_animation_direction(ally.Velocity);
    }

    public override void Exit(){
    }

    public override void When_mouse_right_clicked(Vector2 coords){
        if (!ally.currently_selected){
            return;
        }
        interacted_with_building = simulation_mode.building_to_interact != null;
        if (!interacted_with_building){
            ally.interacted_building = null;
            Set_target_position(coords);
            return;
        }
        ally.interacted_building = simulation_mode.building_to_interact;
        Set_target_position(recalculate_coords(ally.GlobalPosition,ally.interacted_building.GlobalPosition));
        
    }

    public override void When_navigation_finished(){
        if (interacted_with_building){
            switch (ally.interacted_building.data.TYPE){
                case "GreatCommune":
                    Change_state("Taking_shelter");
                    return;
                case "Resource":
                    Change_state("Collecting");
                    return;
            }
        }
        Change_state("Idle");
    }

    public void Check_animation_direction(Vector2 current_velocity){
        // PARA DIREITA E BAIXO
        if (current_velocity.X > 0 && current_velocity.Y > 0){
            ally.sprite.Play("Run down");
            ally.sprite.FlipH = false;
        }
        // PARA A ESQUERDA E BAIXO
        if (current_velocity.X < 0 && current_velocity.Y > 0){
            ally.sprite.Play("Run down");
            ally.sprite.FlipH = true;
        }
        // PARA A ESQUERDA E CIMA
        if (current_velocity.X < 0 && current_velocity.Y < 0){
            ally.sprite.Play("Run up");
            ally.sprite.FlipH = true;
        }
        // PARA A DIREITA E CIMA
        if (current_velocity.X > 0 && current_velocity.Y < 0){
            ally.sprite.Play("Run up");
            ally.sprite.FlipH = false;
        }
    }
}
