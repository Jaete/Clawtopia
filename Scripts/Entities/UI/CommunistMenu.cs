using Godot;

public partial class CommunistMenu : Control
{
	public Ui Ui;
	
	public override void _Ready(){
		Name = Constants.COMMUNIST_MENU;
		Ui = GetNode<Ui>("/root/Game/UI");
		MouseEntered += Ui.Enter_ui_mode;
		MouseExited += Ui.Leave_ui_mode;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
