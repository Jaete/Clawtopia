using Godot;

public partial class MilitaryMenu : Control
{
	public Ui Ui;

	public override void _Ready()
	{
		Ui = GetNode<Ui>("/root/Game/UI");
		MouseEntered += Ui.Enter_ui_mode;
		MouseExited += Ui.Leave_ui_mode;
	}

	public override void _Process(double delta)
	{
	}
}
