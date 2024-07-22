using Godot;
using Godot.Collections;

public partial class EconomicMove : AllyState
{
    public bool is_current_state;
    public override void Enter(){
        is_current_state = true;
    }

    public override void Update(double _delta){
        Move();
    }

    public void Move(){
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
        is_current_state = false;
    }

    public override void When_mouse_right_clicked(Vector2 coords){
        Choose_next_target_position(coords);
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
        if (ally.interacted_resource != null && !ally.delivering){
            GD.Print("COLLECTING");
            Change_state("Collecting");
            return;
        } if (ally.delivering){
            GD.Print("DELIVERING");
            GD.Print("Last pos: ", ally.current_resource_last_position);
            GD.Print("I HAVE: ", ally.resource_current_quantity);
            Set_target_position(ally.current_resource_last_position);
            switch (ally.interacted_resource){
                case CATNIP:
                    ally.level_manager.EmitSignal("ResourceDelivered", CATNIP, ally.resource_current_quantity);
                    break;
                case SALMON:
                    ally.level_manager.EmitSignal("ResourceDelivered", SALMON, ally.resource_current_quantity);
                    break;
                case SAND:
                    ally.level_manager.EmitSignal("ResourceDelivered", SAND, ally.resource_current_quantity);
                    break;
            }
            ally.resource_current_quantity = 0;
            ally.delivering = false;
            GD.Print("my target pos: ", ally.agent.TargetPosition);
            return;
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
