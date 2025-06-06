using Godot;

public partial class Collect : EconomicState
{
    public CollectPoint CurrentlyCollecting;

    [Export] public int MaxQuantity = 3;

    [Export] public int QuantityPerTick = 3;

    public SceneTreeTimer ResourceTickTimer;

    [Export] public float TickTime = 1.0f;

    public override void Enter()
    {
        CurrentlyCollecting = Ally.InteractedResource;
        if (CurrentlyCollecting.ResourceQuantity <= 0)
        {
            ChangeState("Idle");
            return;
        }

        ResourceTickTimer = GetTree().CreateTimer(TickTime); 
        ResourceTickTimer.Timeout += CollectTimeTicked;
    }

    public override void Update(double delta) { }

    public override void Exit()
    {
        if (ResourceTickTimer.GetSignalConnectionList(SceneTreeTimer.SignalName.Timeout).Count > 0)
        {
            ResourceTickTimer.Timeout -= CollectTimeTicked;
        }
        CurrentlyCollecting = null;
    }

    public void CollectTimeTicked()
    {
        var collectedQuantity = SetCollectedQuantity();

        Ally.InteractedResource.EmitSignal(CollectPoint.SignalName.ResourceCollected, collectedQuantity);
        Ally.ResourceCurrentQuantity += collectedQuantity;

        if (Ally.ResourceCurrentQuantity != MaxQuantity && Ally.InteractedResource.ResourceQuantity > 0) {
            ResourceTickTimer = GetTree().CreateTimer(TickTime);
            ResourceTickTimer.Timeout += CollectTimeTicked;
        }
        else 
        {
            var target = GetClosestResourceBuilding(Ally.GlobalPosition, CurrentlyCollecting.ResourceType);
            Ally.Navigation.SetTargetPosition(target.GlobalPosition);
            Ally.Delivering = true;
            ChangeState("Move");
        }
    }

    private int SetCollectedQuantity()
    {
        if (Ally.InteractedResource.ResourceQuantity - QuantityPerTick >= 0)
        {
            if (Ally.ResourceCurrentQuantity + QuantityPerTick < MaxQuantity)
            {
                return QuantityPerTick;
            }
            else
            {
                return MaxQuantity - Ally.ResourceCurrentQuantity;
            }
        }
        else
        {
            if (Ally.ResourceCurrentQuantity + Ally.InteractedResource.ResourceQuantity < MaxQuantity)
            {
                return Ally.InteractedResource.ResourceQuantity;
            }
            else
            {
                return MaxQuantity - Ally.ResourceCurrentQuantity;
            }
        }
    }

    public override void CommandReceived(Vector2 coords)
    {
        if (!Ally.CurrentlySelected) {
            return;
        }

        ChooseNextTargetPosition(coords);
        ChangeState("Move");
    }
}