using Godot;
using System;

public partial class GameHUD : Control
{
    public UI ui;
    public Building building;
    public Button remove_tower_button;

    public override void _Ready() {
        ui = GetNode<UI>("/root/Game/UI");
        remove_tower_button = GetNode<Button>("Button");
        remove_tower_button.Pressed += Remove_tower;
    }

    public void Remove_tower() {
        building.Remove_building_and_rebake();
        building.QueueFree();
        ui.Reset_ui();
    }
}