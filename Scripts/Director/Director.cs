using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Director : Node {
    public bool currently_baking = false;
    public Array<Building> buildings_to_bake = new Array<Building>();
    public NavigationRegion2D region;

    public Godot.Collections.Dictionary game_modes = (Dictionary)new Godot.Collections.Dictionary<string, Variant>();
    public GameMode current_mode;

    public String building_type;
    public int building_count;

    public string tower_type;
    public int tower_count;

    public String resource_build_type;
    public int resource_build_count;

    public override void _Ready() {
        Array<Node> nodes = GetParent().GetChildren();
        foreach (var node in nodes) {
            if(node is GameMode) {
                var gameMode = (GameMode)node;
                gameMode.ModeTransition += change_mode;
                game_modes[gameMode.Name] = gameMode;
            }
        }
        nodes = GetNode("/root/Game/SceneManager/Level").GetChildren();
        foreach (var node in nodes) {
            if (node is Building) {
                building_count++;
                Building building = (Building)node;
                buildings_to_bake.Add(building);
                building.rebake_add_building();
                if (building.data.type == "Tower") { 
                    building.self_index = tower_count;
                    tower_count++;
                }   
            }
        }
        region = GetNode<NavigationRegion2D>("/root/Game/SceneManager/Level/Navigation");
        region.BakeNavigationPolygon();
        buildings_to_bake.Clear();
        current_mode = (GameMode)game_modes["SimulationMode"];
    }

    public void change_mode(String mode, String building, String type) {
        building_type = building;
        switch (building_type) {
            case "Tower":
                tower_type = type;
                break;
            case "Resource":
                resource_build_type = type;
                break;
        }
        current_mode.exit();
        current_mode = (GameMode)game_modes[mode];
        current_mode.enter();
    }

    public override void _Process(double delta) {
        current_mode.update();
    }
}
