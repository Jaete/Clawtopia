using Godot;
    

public partial class PurrlamentMenu: Control
{
    public UI ui;
	
    public override void _Ready(){
        Name = Constants.PURRLAMENT_MENU;
        ui = GetNode<UI>("/root/Game/UI");
        MouseEntered += ui.EnterUiMode;
        MouseExited += ui.ExitUiMode;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}