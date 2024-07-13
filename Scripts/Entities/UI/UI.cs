using Godot;
using System;

public partial class UI : CanvasLayer
{
    public PackedScene building_menu = GD.Load<PackedScene>("res://TSCN/UI/BuildingMenu.tscn");
    public PackedScene base_menu = GD.Load<PackedScene>("res://TSCN/UI/BaseMenu.tscn");

    public Control current_window;

    public Building Building;

    public HFlowContainer container;

    public bool is_resetting_ui;

    public override void _Ready(){
        container = GetNode<HFlowContainer>("HBoxContainer");
    }

    public void Instantiate_window(String window, Building building = null) {
        switch (window) {
            case "BuildingMenu":
                current_window = building_menu.Instantiate<Control>(); 
                if (building != null) {
                    GameHUD game_hud = (GameHUD)current_window;
                    game_hud.building = building;
                    game_hud.Name = "BuildingMenu";
                }
                var base_menu_instance = GetNodeOrNull<Control>("HBoxContainer/GameHUD");
                base_menu_instance?.QueueFree();
                container.AddChild(current_window);
                break;
        }
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("LeftClick")) {
            if (current_window != null && current_window.Name != "GameHUD" && !is_resetting_ui) {
                Reset_ui();
            }
        }
    }

    public void Reset_ui(){
        is_resetting_ui = true;
        current_window.QueueFree();
        current_window = base_menu.Instantiate<Control>();
        current_window.Name = "GameHUD";
        container.AddChild(current_window);
        is_resetting_ui = false;
    }
}
