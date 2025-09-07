using Godot;
using ClawtopiaCs.Scripts.Entities.Characters;

public partial class Collect : EconomicState
{
    public CollectPoint CurrentlyCollecting;

    [Export] public int MaxQuantity = 3;

    [Export] public int QuantityPerTick = 3;

    public SceneTreeTimer ResourceTickTimer;

    [Export] public float TickTime = 1.0f;

    public override void Enter()
    {
        CurrentlyCollecting = Ally.InteractedCollectPoint;
        if (CurrentlyCollecting.ResourceQuantity <= 0)
        {
            ChangeState("Idle");
            return;
        }

        ResourceTickTimer = GetTree().CreateTimer(TickTime);
        ResourceTickTimer.Timeout += CollectTimeTicked;
        PlayCollectAnimation();
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

        Ally.InteractedCollectPoint.EmitSignal(CollectPoint.SignalName.ResourceCollected, collectedQuantity);
        Ally.ResourceCurrentQuantity += collectedQuantity;

        if (Ally.ResourceCurrentQuantity != MaxQuantity && Ally.InteractedCollectPoint.ResourceQuantity > 0)
        {
            ResourceTickTimer = GetTree().CreateTimer(TickTime);
            ResourceTickTimer.Timeout += CollectTimeTicked;
        }
        else
        {
            var target = GetClosestResourceBuilding(Ally.GlobalPosition, CurrentlyCollecting.Resource);
            Ally.Navigation.SetTargetPosition(target.GlobalPosition);
            Ally.Delivering = true;
            ChangeState("Move");
        }
    }

    private int SetCollectedQuantity()
    {
        if (Ally.InteractedCollectPoint.ResourceQuantity - QuantityPerTick >= 0)
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
            if (Ally.ResourceCurrentQuantity + Ally.InteractedCollectPoint.ResourceQuantity < MaxQuantity)
            {
                return Ally.InteractedCollectPoint.ResourceQuantity;
            }
            else
            {
                return MaxQuantity - Ally.ResourceCurrentQuantity;
            }
        }
    }

    public override void CommandReceived(Vector2 coords)
    {
        if (!Ally.CurrentlySelected)
        {
            return;
        }

        ChooseNextTargetPosition(coords);
        ChangeState("Move");
    }
    
    private void PlayCollectAnimation()
    {
        float angle = Mathf.RadToDeg(Ally.LastDirection.Angle());

        if (angle <= 90 && angle > -90)
        {   
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.InteractedCollectPoint.Resource.CollectorAnimationRight);
        }
        else
        {
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.InteractedCollectPoint.Resource.CollectorAnimationLeft);
        }

    }
}