using Godot;
using System;

public partial class GameHUD : Control
{
    public Building building;
    public Button remove_tower_button;

    public override void _Ready() {
        remove_tower_button = GetNode<Button>("Button");
        remove_tower_button.Pressed += Remove_tower;
    }

    public void Remove_tower() {
        building.rebake_remove_building();
        building.rebake();
        building.QueueFree();
        this.QueueFree();
    }
}