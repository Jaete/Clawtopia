using Godot;
using System;

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

	public ModeManager mode_manager;
	public override void _Ready(){
		mode_manager = GetNode<ModeManager>("/root/Game/ModeManager");
	}

	public override void _Input(InputEvent @event){
		var event_button = @event as InputEventMouseButton;
		if (event_button is { Pressed: true, ButtonIndex: MouseButton.Left }){
			EmitSignal("MousePressed", mode_manager.current_level.GetLocalMousePosition());
		}
		if (event_button is { Pressed: false, ButtonIndex: MouseButton.Left }){
			EmitSignal("MouseReleased", mode_manager.current_level.GetLocalMousePosition());
		}
		if (event_button is { DoubleClick: true, ButtonIndex: MouseButton.Left }){
			EmitSignal("MouseDoubleClicked", mode_manager.current_level.GetLocalMousePosition());
		}
		if (event_button is { Pressed: true, ButtonIndex: MouseButton.Right }){
			EmitSignal("MouseRightPressed", mode_manager.current_level.GetLocalMousePosition());
		}
	}
}
