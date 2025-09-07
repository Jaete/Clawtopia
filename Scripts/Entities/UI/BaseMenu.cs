using Godot;

public partial class BaseMenu : Control
{
	
	public override void _Ready(){
		MouseEntered += UI.EnterUIMode;
		MouseExited += UI.EnterUIMode;
	}

}
