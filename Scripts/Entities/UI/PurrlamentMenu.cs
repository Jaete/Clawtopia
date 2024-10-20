using Godot;
    

public partial class PurrlamentMenu: Control
{
    public UI ui;
	
    public override void _Ready(){
        Name = Constants.PURRLAMENT_MENU;
        ui = GetNode<UI>("/root/Game/UI");
        MouseEntered += ui.Enter_ui_mode;
        MouseExited += ui.Leave_ui_mode;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}