using Godot;

public partial class SalmonCollectPoint : CollectPoint
{
    [Export] public AnimatedSprite2D Sprite;

    public override void _Ready()
    {
        base._Ready();
        Sprite.SpriteFrames = Structure.AnimatedTextures;
        ChangeAnimation(ProgressStructure.States.Full);
    }

    public override void ChangeSpriteOnBreakpoint()
    {
        if (ResourceQuantity == 0)
        {
            ChangeAnimation(ProgressStructure.States.Empty);
        }
        else if (ResourceQuantity <= 0.25 * MaxResourceQuantity)
        {
            ChangeAnimation(ProgressStructure.States.Low);
        }
        else if (ResourceQuantity <= 0.75 * MaxResourceQuantity)
        {
            ChangeAnimation(ProgressStructure.States.Medium);
        }
        else if (ResourceQuantity >= 0.75 * MaxResourceQuantity)
        {
            ChangeAnimation(ProgressStructure.States.Full);
        }
    }

    private void ChangeAnimation(ProgressStructure.States state)
    {
        SpriteHandler.ChangeAnimation(Sprite, ProgressStructure.Animations[(int) state], false);
    }
}
