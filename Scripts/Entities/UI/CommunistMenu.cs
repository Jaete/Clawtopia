using Godot;

public partial class CommunistMenu : Control
{
	public UI Ui;
	
	public override void _Ready(){
		Name = Constants.COMMUNIST_MENU;
		Ui = GetNode<UI>("/root/Game/UI");
		MouseEntered += Ui.EnterUiMode;
		MouseExited += Ui.ExitUiMode;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
