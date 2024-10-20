using Godot;

public partial class Move : EconomicState
{
    public override void Enter(){
    }

    public override void Update(double delta){
        Move();
    }

    public override void Exit(){
    }

    public override void MouseRightClicked(Vector2 coords){
        if (!Ally.CurrentlySelected){ return; }
        ChooseNextTargetPosition(coords);
    }

    public override void NavigationFinished(){
        if (Ally.AllyIsBuilding){
            ChangeState("Building");
            return;
        }
        if (Ally.InteractedWithBuilding){
            if (!Ally.InteractedBuilding.IsBuilt){
                Ally.ConstructionToBuild = Ally.InteractedBuilding;
                ChangeState("Building");
                return;
            }
            switch (Ally.InteractedBuilding.Data.Type){
                case Constants.TOWER:
                    ChangeState("Taking_shelter");
                    return;
                case Constants.RESOURCE:
                    SeekResource(Ally.InteractedBuilding.Data.ResourceType);
                    return;
            }
        }
        if (Ally.InteractedResource != null && !Ally.Delivering){
            ChangeState("Collecting");
            return;
        }
        if (Ally.Delivering){
            Ally.Navigation.SetTargetPosition(Ally.CurrentResourceLastPosition);
            switch (Ally.InteractedResource){
                case Constants.CATNIP:
                    Ally.LevelManager.EmitSignal("ResourceDelivered", Constants.CATNIP, Ally.ResourceCurrentQuantity);
                    break;
                case Constants.SALMON:
                    Ally.LevelManager.EmitSignal("ResourceDelivered", Constants.SALMON, Ally.ResourceCurrentQuantity);
                    break;
                case Constants.SAND:
                    Ally.LevelManager.EmitSignal("ResourceDelivered", Constants.SAND, Ally.ResourceCurrentQuantity);
                    break;
            }
            Ally.ResourceCurrentQuantity = 0;
            Ally.Delivering = false;
            return;
        }
        ChangeState("Idle");
    }

    public void SeekResource(string resourceType){
        switch (resourceType){
            case Constants.CATNIP:
                /*TODO implementar*/
                break;
            case Constants.SALMON:
                Ally.CurrentResourceLastPosition = GetClosestWaterCoord();
                Ally.Navigation.SetTargetPosition(Ally.CurrentResourceLastPosition);
                Ally.InteractedResource = Constants.SALMON;
                Ally.InteractedWithBuilding = false;
                break;
            case Constants.SAND:
                /*TODO implementar*/
                break;
        }
    }
}
