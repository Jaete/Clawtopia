using Godot;
using System;

public partial class GameHUD : Control
{
    public Building building;
    public Button remove_tower_button;

    public override void _Ready() {
        remove_tower_button = GetNode<Button>("Button");
        remove_tower_button.Pressed += OnButtonPressed;
    }

    public void remove_tower() {
        GD.Print("Removendo -> ",  building);
        building.rebake_remove_building();
        building.rebake();
        building.QueueFree();
        this.QueueFree();
    }

    public void OnButtonPressed() {
        remove_tower();
    }

}