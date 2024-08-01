using Godot;
using System;

public partial class AddTower : Button
{
    public ModeManager mode_manager;
    public UI ui;
    
    [Export] public BuildingData building;

    public override void _Ready(){
        ui = GetNode<UI>("/root/Game/UI");
        MouseEntered += ui.Enter_ui_mode;
        MouseExited += ui.Leave_ui_mode;
    }

    public void OnPressed() {
        if (mode_manager == null) {
            mode_manager = GetNode<ModeManager>("/root/Game/ModeManager");
        }
        mode_manager.current_mode.EmitSignal("ModeTransition", "BuildMode", building.TYPE, building.NAME);
    }
}
