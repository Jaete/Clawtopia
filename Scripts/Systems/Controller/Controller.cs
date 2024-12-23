using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class Controller : Node
{
    [Signal]
    public delegate void MouseDoubleClickedEventHandler();

    [Signal]
    public delegate void MousePressedEventHandler(Vector2 coords);

    [Signal]
    public delegate void MouseReleasedEventHandler(Vector2 coords);

    [Signal]
    public delegate void MouseRightPressedEventHandler(Vector2 coords);

    public ModeManager ModeManager;

    public override void _Ready()
    {
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
    }

    public override void _Input(InputEvent @event)
    {
        var eventButton = @event as InputEventMouseButton;

        if (eventButton is { Pressed: true, ButtonIndex: MouseButton.Left })
        {
            EmitSignal(SignalName.MousePressed, ModeManager.CurrentLevel.GetLocalMousePosition());
        }

        if (eventButton is { Pressed: false, ButtonIndex: MouseButton.Left })
        {
            EmitSignal(SignalName.MouseReleased, ModeManager.CurrentLevel.GetLocalMousePosition());
        }

        if (eventButton is { DoubleClick: true, ButtonIndex: MouseButton.Left })
        {
            EmitSignal(SignalName.MouseDoubleClicked, ModeManager.CurrentLevel.GetLocalMousePosition());
        }

        if (eventButton is { Pressed: true, ButtonIndex: MouseButton.Right })
        {
            EmitSignal(SignalName.MouseRightPressed, ModeManager.CurrentLevel.GetLocalMousePosition());
        }
    }
}