using Godot;
using System;

public partial class AddTower : Button
{
    public Director director;

    
    public BuildingData tower;

    public override void _Ready() {
        director = (Director)GetNode("root/Game/Director");
    }
}
