using Godot;
using System;

public partial class BaseMenu : Control
{
    public UI ui;
	
	public override void _Ready(){
		ui = GetNode<UI>("/root/Game/UI");
		MouseEntered += ui.Enter_ui_mode;
		MouseExited += ui.Leave_ui_mode;
	}

}
