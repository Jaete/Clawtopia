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
        CurrentlyCollecting = Ally.InteractedResource;
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

        Ally.InteractedResource.EmitSignal(CollectPoint.SignalName.ResourceCollected, collectedQuantity);
        Ally.ResourceCurrentQuantity += collectedQuantity;

        if (Ally.ResourceCurrentQuantity != MaxQuantity && Ally.InteractedResource.ResourceQuantity > 0)
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
            if (Ally.InteractedResource.ResourceType == Constants.CATNIP)
                SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.CatnipRight], false);
            else if (Ally.InteractedResource.ResourceType == Constants.SAND)
                SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.SandRight], false);
            else if (Ally.InteractedResource.ResourceType == Constants.SALMON)
                SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.FishingRight], false);
        }
        else
            if (Ally.InteractedResource.ResourceType == Constants.CATNIP)
                SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.CatnipLeft], false);
            else if (Ally.InteractedResource.ResourceType == Constants.SAND)
                SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.SandLeft], false);
            else if (Ally.InteractedResource.ResourceType == Constants.SALMON)
                SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.FishingLeft], false);
    }
}