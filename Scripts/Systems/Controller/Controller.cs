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
        var eventButton = @event is InputEventMouseButton ? @event as InputEventMouseButton : null;
        var eventKey = @event is InputEventKey ?@event as InputEventKey : null;

        if (NotHandledInput(@event)) { return; }

        if (eventButton != null) { 
            if (eventButton is { Pressed: true, ButtonIndex: MouseButton.Left })
            {
                EmitSignal(SignalName.MousePressed, ModeManager.CurrentLevel.GetLocalMousePosition());
            }
            else if (eventButton is { Pressed: false, ButtonIndex: MouseButton.Left })
            {
                EmitSignal(SignalName.MouseReleased, ModeManager.CurrentLevel.GetLocalMousePosition());
            }
            else if (eventButton is {  DoubleClick: true, ButtonIndex: MouseButton.Left })
            {
                EmitSignal(SignalName.MouseDoubleClicked, ModeManager.CurrentLevel.GetLocalMousePosition());
            }
            else if (eventButton is { Pressed: false, ButtonIndex: MouseButton.Right })
            {
                EmitSignal(SignalName.MouseRightPressed, ModeManager.CurrentLevel.GetLocalMousePosition());
            }
        } 
        else if (eventKey != null)
        {
            if (eventKey.Pressed && eventKey.Keycode == Key.R)
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