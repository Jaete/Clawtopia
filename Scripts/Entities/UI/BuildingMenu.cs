using Godot;
using System;

public partial class BuildingMenu : Control
{
    public UI ui;
    public Building building;
    public Button remove_tower_button;
    public ModeManager mode_manager;

    public override void _Ready() {
        Name = Constants.BUILDING_MENU;
        mode_manager = GetNode<ModeManager>("/root/Game/ModeManager");
        ui = GetNode<UI>("/root/Game/UI");
        remove_tower_button = GetNode<Button>("Button");
        remove_tower_button.Pressed += Remove_tower;
        MouseEntered += ui.Enter_ui_mode;
        MouseExited += ui.Leave_ui_mode;
    }

    public void Remove_tower() {
        building.Remove_building_and_rebake();
        building.QueueFree();
        ui.Reset_ui();
    }
}