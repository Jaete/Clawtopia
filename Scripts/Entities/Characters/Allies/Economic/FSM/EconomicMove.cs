using Godot;
using Godot.Collections;

public partial class EconomicMove : State
{
    public override void Enter(){
    }

    public override void Update(double _delta){
        Move();
    }

    public override void Exit(){
    }

    public override void When_mouse_right_clicked(Vector2 coords){
        if (!self.currently_selected){ return; }
        Choose_next_target_position_ECONOMIC(coords);
    }

    public override void When_navigation_finished(){
        if (self.ally_is_building){
            Change_state("Building");
            return;
        }
        if (self.interacted_with_building){
            if (!self.interacted_building.is_built){
                self.construction_to_build = self.interacted_building;
                Change_state("Building");
                return;
            }
            switch (self.interacted_building.data.TYPE){
                case Constants.TOWER:
                    Change_state("Taking_shelter");
                    return;
                case Constants.RESOURCE:
                    Seek_resource(self.interacted_building.data.RESOURCE_TYPE);
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
                case Constants.CATNIP:
                    self.level_manager.EmitSignal("ResourceDelivered", Constants.CATNIP, self.resource_current_quantity);
                    break;
                case Constants.SALMON:
                    self.level_manager.EmitSignal("ResourceDelivered", Constants.SALMON, self.resource_current_quantity);
                    break;
                case Constants.SAND:
                    self.level_manager.EmitSignal("ResourceDelivered", Constants.SAND, self.resource_current_quantity);
                    break;
            }
            self.resource_current_quantity = 0;
            self.delivering = false;
            return;
        }
        Change_state("Idle");
    }

    public void Seek_resource(string resource_type){
        switch (resource_type){
            case Constants.CATNIP:
                /*TODO implementar*/
                break;
            case Constants.SALMON:
                self.current_resource_last_position = Get_closest_water_coord();
                Set_target_position(self.current_resource_last_position);
                self.interacted_resource = Constants.SALMON;
                self.interacted_with_building = false;
                break;
            case Constants.SAND:
                /*TODO implementar*/
                break;
        }
    }
}
