using Godot;
using Godot.Collections;
using System;

public partial class ModeManager : Node2D
{
    public bool currently_baking = false;
    public Array<Building> buildings_to_bake = new();
    public NavigationRegion2D region;

    public Dictionary game_modes = (Dictionary) new Dictionary<string, Variant>();
    public GameMode current_mode;
    public int building_count;

    public string tower_type;
    
    public int fighters_tower_count;
    public int great_commune_count;
    public int salmon_cottage_count;

    public String resource_build_type;
    public int resource_build_count;

    public int house_count;

    public Director director;
    public Node2D current_level;

    public override void _Ready() {
        Initialize();
    }

    public void Initialize() {
        director = GetNode<Director>("/root/Game/Director");
        current_level = GetNode<Node2D>("/root/Game/LevelManager/Level");
        Set_game_modes();
        Set_initial_buildings();
        current_mode = (GameMode) game_modes["SimulationMode"];
    }

    public void Change_mode(String mode, String building, String type) {
        current_mode.Exit();
        current_mode = (GameMode)game_modes[mode];
        if (current_mode is BuildMode build_mode) {
            build_mode.building_type = building;
            switch (build_mode.building_type) {
                case Constants.TOWER:
                    tower_type = type;
                    break;
                case Constants.RESOURCE:
                    resource_build_type = type;
                    break;
            }
        }
        current_mode.Enter();
    }

    public override void _Process(double delta) {
        current_mode.Update();
    }

    public void Set_game_modes(){
        Array<Node> modes = GetChildren();
        foreach (var mode in modes) {
            if (mode is GameMode game_mode) {
                game_mode.ModeTransition += Change_mode;
                game_modes[game_mode.Name] = game_mode;
            }
        }
    }

    public void Set_initial_buildings(){
        Array<Node> nodes = GetNode("/root/Game/LevelManager/Level").GetChildren();
        foreach (var node in nodes) {
            if (node is Building build){
                building_count++;
                Building building = build;
                if (building.data.TYPE == Constants.TOWER) {
                    if (building.Name.ToString().Contains(Constants.FIGHTERS)){
                        building.self_index = fighters_tower_count;
                        fighters_tower_count++;
                    }
                }
                if (building.data.TYPE == Constants.RESOURCE) { 
                    if (building.Name.ToString().Contains(Constants.COMMUNIST)){
                        building.self_index = salmon_cottage_count;
                        salmon_cottage_count++;
                    }
                }
                if (building.data.TYPE == Constants.COMMUNE) { 
                    building.self_index = great_commune_count;
                    great_commune_count++;
                }
                buildings_to_bake.Add(building);
                building.Rebake_add_building();
            }
        }
        region = GetNode<NavigationRegion2D>("/root/Game/LevelManager/Level/Navigation");
        region.BakeNavigationPolygon();
        buildings_to_bake.Clear();
    }

}
