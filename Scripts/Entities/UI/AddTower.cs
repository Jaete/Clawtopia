using Godot;
using System;

public partial class AddTower : Button
{
    public ModeManager mode_manager;
    
    [Export] public BuildingData building;

    public void OnPressed() {
        if (mode_manager == null) {
            mode_manager = GetNode<ModeManager>("/root/Game/ModeManager");
        }
        mode_manager.current_mode.EmitSignal("ModeTransition", "BuildMode", building.type, building.name);
    }
}
