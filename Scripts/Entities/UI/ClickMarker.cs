using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;
using Godot.Collections;

public partial class ClickMarker : Node2D
{
    [Export]
    public SpriteFrames ClickAnimation;

    private Array<AnimatedSprite2D> Markers = [];

    public override void _Ready()
    {
        SimulationMode.Singleton.AllyCommand += OnMousePressed;
        CallDeferred(MethodName.Reparent, ModeManager.Singleton.CurrentLevel);
    }

    public void OnMousePressed(Vector2 coords)
    {
        var marker = CreateNewMarker(coords);
        Markers.Add(marker);
        marker.AnimationFinished += MarkerAnimationFinished;
        ModeManager.Singleton.CurrentLevel.AddChild(marker);
        marker.GlobalPosition = coords;
        marker.Play(ClickAnimation.GetAnimationNames()[0]);
    }

    private void MarkerAnimationFinished()
    {
        Markers[0].QueueFree();
        Markers.RemoveAt(0);
    }

    private AnimatedSprite2D CreateNewMarker(Vector2 coords)
    {
        return new AnimatedSprite2D
        {
            SpriteFrames = ClickAnimation,
            Visible = true,
        };
    }
}
