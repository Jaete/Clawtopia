using Godot;
using Godot.Collections;
using System;

public partial class ModeManager : Node2D
{
    public bool currently_baking = false;
    public Array<Building> buildings_to_bake = new Array<Building>();
    public NavigationRegion2D region;

    public Godot.Collections.Dictionary game_modes = (Dictionary)new Godot.Collections.Dictionary<string, Variant>();
    public GameMode current_mode;
    public int building_count;

    public string tower_type;
    public int tower_count;

    public String resource_build_type;
    public int resource_build_count;

    public Director director;
    public Node2D current_level;

    public override void _Ready() {
        Initialize();
    }

    public void Initialize() {
        director = (Director)GetNode("/root/Game/Director");
        current_level = (Node2D)GetNode("/root/Game/LevelManager/Level");
        Set_game_modes();
        Set_initial_buildings();
        current_mode = (GameMode)game_modes["SimulationMode"];
    }

    public void Change_mode(String mode, String building, String type) {
        current_mode.exit();
        current_mode = (GameMode)game_modes[mode];
        if(current_mode is BuildMode){
            BuildMode build_mode = (BuildMode)current_mode;
            build_mode.building_type = building;
            switch (build_mode.building_type) {
            case "Tower":
                tower_type = type;
                break;
            case "Resource":
                resource_build_type = type;
                break;
            }
        }
        current_mode.enter();
    }

    public override void _Process(double delta) {
        current_mode.update();
    }

    public void Set_game_modes(){
        Array<Node> modes = GetChildren();
        foreach (var mode in modes) {
            if(mode is GameMode) {
                var gameMode = (GameMode)mode;
                gameMode.ModeTransition += Change_mode;
                game_modes[gameMode.Name] = gameMode;
            }
        }
    }

    public void Set_initial_buildings(){
        Array<Node> nodes = GetNode("/root/Game/LevelManager/Level").GetChildren();
        foreach (var node in nodes) {
            if (node is Building build){
                building_count++;
                Building building = build;
                buildings_to_bake.Add(building);
                building.rebake_add_building();
                if (building.data.type == "Tower") { 
                    building.self_index = tower_count;
                    tower_count++;
                }   
            }
        }
        region = GetNode<NavigationRegion2D>("/root/Game/LevelManager/Level/Navigation");
        region.BakeNavigationPolygon();
        buildings_to_bake.Clear();
    }

}
