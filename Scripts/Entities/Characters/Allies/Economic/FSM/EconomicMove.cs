using Godot;
using Godot.Collections;

public partial class EconomicMove : State
{
    public bool is_current_state;
    public override void Enter(){
        is_current_state = true;
    }

    public override void Update(double _delta){
        Move();
    }

    public void Move(){
        var next_path_pos = self.agent.GetNextPathPosition();
        var new_velocity = self.GlobalPosition.DirectionTo(next_path_pos);
        if (self.agent.AvoidanceEnabled){
            self.agent.Velocity = new_velocity.Normalized();
        }else{
            When_velocity_computed(new_velocity.Normalized());
        }
        if (Input.IsActionPressed("RightClick") && self.currently_selected){
            Set_target_position(self.GetGlobalMousePosition());
        }
        Check_animation_direction(self.Velocity);
    }

    public override void Exit(){
        is_current_state = false;
    }

    public override void When_mouse_right_clicked(Vector2 coords){
        if (!self.currently_selected){ return; }
        Choose_next_target_position(coords);
    }

    public override void When_navigation_finished(){
        if (self.ally_is_building){
            Change_state("Building");
            return;
        }
        if (self.interacted_with_building){
            switch (self.interacted_building.data.TYPE){
                case "GreatCommune":
                    Change_state("Taking_shelter");
                    return;
                case "Resource":
                    Change_state("Collecting");
                    return;
            }
            if (!self.interacted_building.is_built){
                self.construction_to_build = self.interacted_building;
                Change_state("Building");
                return;
            }
        }
        if (self.interacted_resource != null && !self.delivering){
            Change_state("Collecting");
            return;
        }
        if (self.delivering){
            Set_target_position(self.current_resource_last_position);
            switch (self.interacted_resource){
                case CATNIP:
                    self.level_manager.EmitSignal("ResourceDelivered", CATNIP, self.resource_current_quantity);
                    break;
                case SALMON:
                    self.level_manager.EmitSignal("ResourceDelivered", SALMON, self.resource_current_quantity);
                    break;
                case SAND:
                    self.level_manager.EmitSignal("ResourceDelivered", SAND, self.resource_current_quantity);
                    break;
            }
            self.resource_current_quantity = 0;
            self.delivering = false;
            return;
        }
        Change_state("Idle");
    }

    public void Check_animation_direction(Vector2 current_velocity){
        // PARA DIREITA E BAIXO
        if (current_velocity.X > 0 && current_velocity.Y > 0){
            self.sprite.Play("Run down");
            self.sprite.FlipH = false;
        }
        // PARA A ESQUERDA E BAIXO
        if (current_velocity.X < 0 && current_velocity.Y > 0){
            self.sprite.Play("Run down");
            self.sprite.FlipH = true;
        }
        // PARA A ESQUERDA E CIMA
        if (current_velocity.X < 0 && current_velocity.Y < 0){
            self.sprite.Play("Run up");
            self.sprite.FlipH = true;
        }
        // PARA A DIREITA E CIMA
        if (current_velocity.X > 0 && current_velocity.Y < 0){
            self.sprite.Play("Run up");
            self.sprite.FlipH = false;
        }
    }
}
