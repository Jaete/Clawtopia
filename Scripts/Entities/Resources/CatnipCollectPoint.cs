using Godot;

public partial class CatnipCollectPoint : CollectPoint
{ 

    [Export] public Sprite2D Sprite;

    public override void _Ready()
    {
        base._Ready();
        ChangeStaticSprite(ProgressStructure.States.Full);
    }

    public override void ChangeSpriteOnBreakpoint()
    {
        if (ResourceQuantity == 0)
        {
            ChangeStaticSprite(ProgressStructure.States.Empty);
        }
        else if (ResourceQuantity <= 0.25 * MaxResourceQuantity)
        {
            ChangeStaticSprite(ProgressStructure.States.Low);
        }
        else if (ResourceQuantity <= 0.75 * MaxResourceQuantity)
        {
            ChangeStaticSprite(ProgressStructure.States.Medium);
        }
        else if (ResourceQuantity >= 0.75 * MaxResourceQuantity)
        {
            ChangeStaticSprite(ProgressStructure.States.Full);
        }
    }

    private void ChangeStaticSprite(ProgressStructure.States state)
    {
        SpriteHandler.ChangeSprite(Sprite, Structure.StaticTextures[(int)state]);
    }
}
