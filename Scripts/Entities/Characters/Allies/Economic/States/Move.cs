using ClawtopiaCs.Scripts.Systems;
using Godot;
using Godot.Collections;

public partial class Move : EconomicState
{
    public override void Enter() {}

    public override void Update(double delta)
    {
        Move();
    }

    public override void Exit() {}

    public override void MouseRightClicked(Vector2 coords)
    {
        if (!Ally.CurrentlySelected || ModeManager.CurrentMode is BuildMode)
        {
            return;
        }

        ChooseNextTargetPosition(coords);
    }

    public override void NavigationFinished()
    {
        if (Ally.AllyIsBuilding)
        {
            ChangeState("Building");
            return;
        }

        if (Ally.InteractedWithBuilding)
        {
            if (!Ally.InteractedBuilding.IsBuilt)
            {
                Ally.ConstructionToBuild = Ally.InteractedBuilding;
                ChangeState("Building");
                return;
            }

            switch (Ally.InteractedBuilding.Data.Type)
            {
                case Constants.TOWER:
                    ChangeState("Taking_shelter");
                    return;
                case Constants.RESOURCE:
                    SeekResource(Ally.InteractedBuilding.Data.ResourceType);
                    return;
            }
        }

        if (Ally.InteractedResource != null && !Ally.Delivering)
        {
            ChangeState("Collecting");
            return;
        }

        if (Ally.Delivering)
        {
            Ally.Navigation.SetTargetPosition(Ally.CurrentResourceLastPosition);
            var resources = new Dictionary<string, int>();

            switch (Ally.InteractedResource)
            {
                case Constants.CATNIP:
                    resources.Add(Constants.CATNIP, Ally.ResourceCurrentQuantity);
                    Ally.LevelManager.EmitSignal(LevelManager.SignalName.ResourceDelivered, resources);
                    break;
                case Constants.SALMON:
                    resources.Add(Constants.SALMON, Ally.ResourceCurrentQuantity);
                    Ally.LevelManager.EmitSignal(LevelManager.SignalName.ResourceDelivered, resources);
                    break;
                case Constants.SAND:
                    resources.Add(Constants.SAND, Ally.ResourceCurrentQuantity);
                    Ally.LevelManager.EmitSignal(LevelManager.SignalName.ResourceDelivered, resources);
                    break;
            }

            Ally.ResourceCurrentQuantity = 0;
            Ally.Delivering = false;
            return;
        }

        ChangeState("Idle");
    }

    public void SeekResource(string resourceType)
    {
        Ally.CurrentResourceLastPosition = GetClosestResourceCoord(resourceType);
        Ally.Navigation.SetTargetPosition(Ally.CurrentResourceLastPosition);
        Ally.InteractedResource = resourceType;
        Ally.InteractedWithBuilding = false;
    }
}