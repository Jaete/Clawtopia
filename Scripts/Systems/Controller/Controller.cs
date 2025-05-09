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

    [Signal]
    public delegate void RotateBuildingEventHandler();

    public ModeManager ModeManager;

    public override void _Ready()
    {
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        var eventButton = @event as InputEventMouseButton;
        var eventKey = @event as InputEventKey;

        if (NotHandledInput(@event)) { return; }

        if (eventButton != null || eventKey != null) { 
            if (eventButton is { Pressed: true, ButtonIndex: MouseButton.Left })
            {
                EmitSignal(SignalName.MousePressed, ModeManager.CurrentLevel.GetLocalMousePosition());
            }
            else if (eventButton is { Pressed: false, ButtonIndex: MouseButton.Left })
            {
                EmitSignal(SignalName.MouseReleased, ModeManager.CurrentLevel.GetLocalMousePosition());
            }
            else if (eventButton is { DoubleClick: true, ButtonIndex: MouseButton.Left })
            {
                EmitSignal(SignalName.MouseDoubleClicked, ModeManager.CurrentLevel.GetLocalMousePosition());
            }
            else if (eventButton is { Pressed: true or false, ButtonIndex: MouseButton.Right })
            {
                EmitSignal(SignalName.MouseRightPressed, ModeManager.CurrentLevel.GetLocalMousePosition());
            }
            else if (eventKey.Pressed && eventKey.Keycode == Key.R && ModeManager.CurrentMode is BuildMode)
            {
                EmitSignal(SignalName.RotateBuilding);
            }
        }

    }

    public bool NotHandledInput(InputEvent @event)
    {
        var isWheel = (
            @event as InputEventMouseButton is { ButtonIndex: MouseButton.WheelDown } ||
            @event as InputEventMouseButton is { ButtonIndex: MouseButton.WheelUp } ||
            @event as InputEventMouseButton is { ButtonIndex: MouseButton.Middle }
        );

        var isNotKey = @event as InputEventKey is null;
        
        var isNotClick = @event as InputEventMouseButton is null;

        return (isNotKey && isNotClick) || isWheel;
    }
}