using Godot;
using System;

public partial class UI : CanvasLayer
{
    PackedScene tower_menu = GD.Load<PackedScene>("res://TSCN/UI/GameHUD.tscn");

    Control current_window = null;

    Building Building;

    public void instantiate_window(String window, Building building = null) {
        switch (window) {
            case "TowerMenu":
                current_window = tower_menu.Instantiate<Control>(); 
                if (building != null) {
                    GameHUD gameHUD = (GameHUD)current_window;
                    gameHUD.building = building;
                }
                AddChild(current_window);
                break;
        }
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("LeftClick")) {
            if (current_window != null && current_window.Name != "Main") {
                current_window.QueueFree();
                current_window = null;
            }
        }
    }
}
