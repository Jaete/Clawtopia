using Godot;
using System;

public partial class BuildMode : GameMode {


    public Color OK_COLOR = new Color("2eff3f81");
    public Color ERROR_COLOR = new Color("ba000079");
    public Color REGULAR_COLOR = new Color(1, 1, 1, 1);

    public const int TILE_SIZE_X = 48;
    public const int TILE_SIZE_Y = 24;

    public Vector2 mouse_position;

    public Building current_building;
    public bool is_overlapping_buildings = false;

    public override void _Ready() {
        initialize();
    }
    public override void enter() {
        String building_path;
        switch (director.building_type) {
            case "Tower":
                building_path = "res://TSCN/Objects/Buildings/Towers/" + director.tower_type + ".tscn";
                instantiate_building(building_path);
                director.tower_count++;
                director.building_count++;
                current_building.Name = director.tower_type + "_T1(" + director.tower_count + ")";
                break;
            case "Commune":
                building_path = "res://TSCN/Objects/Buildings/Base/GreatCommune.tscn";
                instantiate_building(building_path);
                director.building_count++;
                current_building.Name = "GreatCommune";
                break;
            case "Resource":
                building_path = "res://TSCN/Objects/Buildings/Resource/" + director.resource_build_type + ".tscn";
                instantiate_building(building_path);
                director.resource_build_count++;
                current_building.Name = "" + director.resource_build_type + "_" + director.resource_build_count;
                break;
        }
        director.AddChild(current_building);
        mouse_position = current_level.GetGlobalMousePosition();
        current_building.GlobalPosition = mouse_position;
    }

    public override void update() {
        move_preview();
        validate_position();
        if (Input.IsActionJustPressed("LeftClick")) {
            confirm_building();
        }
        if (Input.IsActionJustPressed("RightClick")) {
            cancel_building();
        }
    }

    private void cancel_building() {
        current_building.QueueFree();
        EmitSignal("ModeTransition", "SimulationMode");
    }

    private void confirm_building() {
        if (!is_overlapping_buildings) {
            current_building.rebake_add_building();
            current_building.Modulate = REGULAR_COLOR;
            EmitSignal("ModeTransition", "SimulationMode");
        }
    }

    private void validate_position() {
        Area2D grid_area = current_building.GetNode<Area2D>("GridArea");
        Godot.Collections.Array<Area2D> overlapping_areas = grid_area.GetOverlappingAreas();
        foreach (var area in overlapping_areas) {
            if (area.Name == "GridArea") {
                is_overlapping_buildings = true;
                break;
            }
        }
        if (is_overlapping_buildings) {
            current_building.Modulate = ERROR_COLOR;
        } else {
            current_building.Modulate = OK_COLOR;
        }
    }

    private void move_preview() {
        mouse_position = current_level.GetGlobalMousePosition();
        float x_difference = mouse_position.X - current_building.GlobalPosition.X;
        float y_difference = mouse_position.Y + current_building.GlobalPosition.Y;
        float new_x = 0;
        float new_y = 0;
        if (x_difference >= (TILE_SIZE_X / 2)) {
            new_x = (TILE_SIZE_X / 2);

        } else if (x_difference <= (TILE_SIZE_X / 2) * -1) {
            new_x = (TILE_SIZE_X / 2) * -1;
        }
        if (y_difference >= (TILE_SIZE_Y / 2)) {
            new_y = (TILE_SIZE_Y / 2);
        } else if (y_difference <= (TILE_SIZE_Y / 2) * -1) {
            new_y = (TILE_SIZE_Y / 2) * -1;
        }
        current_building.GlobalPosition = new Vector2(
            current_building.GlobalPosition.X + new_x,
            current_building.GlobalPosition.Y + new_y
        );

    }

    private void instantiate_building(String building_path) {
        PackedScene building_scene = (PackedScene)ResourceLoader.Load(building_path);
        current_building = (Building)building_scene.Instantiate();
    }
}
