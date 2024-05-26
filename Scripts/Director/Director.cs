using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Director : Node {
    public bool currently_baking = false;
    public Array<int> buildings_to_bake = new Array<int>();

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
                buildings_to_bake.Add(building_count);
                Building building = (Building)node;
                if (building.data.type == "Tower") { 
                    building.self_index = tower_count;
                    tower_count++;
                }   
            }
        }
        GD.Print("Build to bake ->", buildings_to_bake);
        current_mode = (GameMode)game_modes["SimulationMode"];
    }

    public void change_mode(String mode, String building, String type) {
        building_type = building;
        tower_type = type;
        current_mode.exit();
        current_mode = (GameMode)game_modes[mode];
        current_mode.enter();
    }

    public override void _Process(double delta) {
        current_mode.update();
    }
}
