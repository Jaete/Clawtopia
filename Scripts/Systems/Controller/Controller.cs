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
		if (event_button != null && event_button.Pressed && event_button.ButtonIndex == MouseButton.Left){
			EmitSignal("MousePressed", mode_manager.current_level.GetGlobalMousePosition());
		}
		if (event_button != null && !event_button.Pressed && event_button.ButtonIndex == MouseButton.Left){
			EmitSignal("MouseReleased", mode_manager.current_level.GetGlobalMousePosition());
		}
		if (event_button != null && event_button.DoubleClick && event_button.ButtonIndex == MouseButton.Left){
			EmitSignal("MouseDoubleClicked", mode_manager.current_level.GetGlobalMousePosition());
		}
		if (event_button != null && event_button.Pressed && event_button.ButtonIndex == MouseButton.Right){
			EmitSignal("MouseRightPressed", mode_manager.current_level.GetGlobalMousePosition());
		}
	}
}
