using Godot;
using System;

public partial class AddTower : Button
{
    public Director director;
    
    [Export] public BuildingData building;

    public void OnPressed() {
        if (director == null) {
            director = GetNode<Director>("/root/Game/Director");
        }
        director.current_mode.EmitSignal("ModeTransition", "BuildMode", building.type, building.name);
    }
}
