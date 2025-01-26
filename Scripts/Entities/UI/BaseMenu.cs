using Godot;

public partial class BaseMenu : Control
{
    public UI Ui;
	
	public override void _Ready(){
		Ui = GetNode<UI>("/root/Game/UI");
		MouseEntered += Ui.EnterUiMode;
		MouseExited += Ui.ExitUiMode;
	}

}
