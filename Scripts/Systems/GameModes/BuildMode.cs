using Godot;
using System;

public partial class BuildMode : GameMode {

    public int TILE_SIZE_X = 48;
    public int TILE_SIZE_Y = 24;

    public Vector2 mouse_position;

    public String building_type;

    public Building current_building;
    public bool is_overlapping_buildings = false;
    public override void Enter() {
        String building_path = "res://TSCN/Entities/Buildings/Building.tscn";
        if(building_type == "Tower") {
            mode_manager.fighters_tower_count++;
            mode_manager.building_count++;
            Instantiate_building(building_path);
            current_building.self_index = mode_manager.fighters_tower_count;
            current_building.Name = mode_manager.tower_type + "_T1_" + current_building.self_index;
            current_building.data = GD.Load<BuildingData>("res://Resources/Buildings/Towers/Fighters/Fighters.tres");
        }
        if(building_type == "Commune"){
           mode_manager.great_commune_count++;
           mode_manager.building_count++;
           Instantiate_building(building_path);
           current_building.self_index = mode_manager.great_commune_count;
           current_building.Name = "GreatCommune";   
           current_building.data = GD.Load<BuildingData>("res://Resources/Buildings/GreatCommune/GreatCommune.tres");
        }    
        if(building_type == "Resource"){
            mode_manager.salmon_cottage_count++;
            mode_manager.building_count++;
            Instantiate_building(building_path);
            current_building.self_index = mode_manager.salmon_cottage_count;
            current_building.Name = "" + mode_manager.resource_build_type + "_" + current_building.self_index;
        }
        current_building.InputPickable = false;
        mode_manager.current_level.AddChild(current_building);
        mouse_position = mode_manager.current_level.GetGlobalMousePosition();
        current_building.GlobalPosition = mouse_position;
    }

    public override void Update() {
        Move_preview();
        Validate_position();
        if (Input.IsActionJustPressed("LeftClick")) {
            Confirm_building();
        }
        if (Input.IsActionJustPressed("RightClick")) {
            Cancel_building();
        }
    }

    private void Cancel_building() {
        mode_manager.fighters_tower_count--;
        mode_manager.building_count--;
        current_building.QueueFree();
        EmitSignal("ModeTransition", "SimulationMode", "", "");
    }

    private void Confirm_building() {
        if (!is_overlapping_buildings) {
            current_building.Rebake_add_building();
            current_building.Modulate = current_building.REGULAR_COLOR;
            current_building.Rebake();
            current_building.InputPickable = true;
            EmitSignal("ModeTransition", "SimulationMode", "", "");
        }
    }

    private void Validate_position() {
        Area2D grid_area = current_building.GetNode<Area2D>("GridArea");
        Godot.Collections.Array<Area2D> overlapping_areas = grid_area.GetOverlappingAreas();
        foreach (var area in overlapping_areas) {
            if (area.Name == "GridArea") {
                is_overlapping_buildings = true;
                break;
            }
        }
        if(overlapping_areas.Count == 0) {
            is_overlapping_buildings = false;    
        }
        if (is_overlapping_buildings) {
            current_building.Modulate = current_building.ERROR_COLOR;
        } else {
            current_building.Modulate = current_building.OK_COLOR;
        }
    }

    private void Move_preview() {
        mouse_position = mode_manager.current_level.GetGlobalMousePosition();
        float x_difference = mouse_position.X - current_building.GlobalPosition.X;
        float y_difference = mouse_position.Y - current_building.GlobalPosition.Y;
        float new_x = 0;
        float new_y = 0;
        if (x_difference > (TILE_SIZE_X / 2)) {
            new_x = (TILE_SIZE_X / 2);

        } else if (x_difference < (TILE_SIZE_X / 2) * -1) {
            new_x = (TILE_SIZE_X / 2) * -1;
        }
        if (y_difference > (TILE_SIZE_Y / 2)) {
            new_y = (TILE_SIZE_Y / 2);
        } else if (y_difference < (TILE_SIZE_Y / 2) * -1) {
            new_y = (TILE_SIZE_Y / 2) * -1;
        }
        current_building.GlobalPosition = new Vector2(
            current_building.GlobalPosition.X + new_x,
            current_building.GlobalPosition.Y + new_y
        );

    }

    private void Instantiate_building(String building_path) {
        PackedScene building_scene = GD.Load<PackedScene>(building_path);
        current_building = (Building)building_scene.Instantiate();
    }
}
