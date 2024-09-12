using Godot;

public partial class BaseMenu : Control
{
    public UI Ui;
	
	public override void _Ready(){
		Ui = GetNode<UI>("/root/Game/UI");
		MouseEntered += Ui.Enter_ui_mode;
		MouseExited += Ui.Leave_ui_mode;
	}

}
