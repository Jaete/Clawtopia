using Godot;
using System;
using System.IO;

public partial class Building : Area2D {
    
    public NavigationRegion2D region;
    public BuildMode building_mode;

    public ModeManager mode_manager;
    
    public int self_index;
    public bool placed;

    [Export] public bool is_pre_spawned = false;
    [Export] public BuildingData data;
    [Export] public StaticBody2D static_body;
    [Export] public CollisionPolygon2D body_shape;
    [Export] public CollisionShape2D interaction_shape;
    [Export] public CollisionPolygon2D grid_shape;
    [Export] public Sprite2D sprite;


    public override void _Ready() {
        initialize();
    }

    public void initialize() {
        mode_manager = GetNode<ModeManager>("/root/Game/ModeManager");
        building_mode = GetNode<BuildMode>("/root/Game/ModeManager/BuildMode");
        region = GetNode<NavigationRegion2D>("../Navigation");
        static_body = GetNode<StaticBody2D>("NavigationBody");
        data.initialize();
        body_shape.Polygon = data.obstacle_shape.Segments;
        interaction_shape.Shape = data.interaction_shape;
        grid_shape.Polygon = data.grid_shape.Segments;
        sprite.Texture = data.sprite_texture;
        sprite.Offset = data.sprite_offset;
        sprite.Scale = data.sprite_size;
        interaction_shape.Position = data.interaction_offset;
        if(data.type.Equals("GreatCommune")) {
            Name = data.type;
        } else {
            Name = data.name + "_" + data.type + "_" + self_index;
        }
        region.BakeFinished += free_to_rebake;
    }

    public void set_rebake() {
        static_body.Name = "Obstacle_Region_" + data.type + "_" + self_index;
        static_body.Reparent(region);
        mode_manager.currently_baking = true;
    }

    public void rebake_add_building() {
        if (is_pre_spawned) {
            set_rebake();
            return;
        }
        if (mode_manager.current_mode is BuildMode) {
            set_rebake();
            return;
        }
    }

    public void rebake_remove_building() {
        if (!mode_manager.currently_baking) {
            StaticBody2D obstacle = (StaticBody2D)region.GetNode("Obstacle_Region_" + data.type + "_" + self_index);
            obstacle.Reparent(this);
            region.BakeNavigationPolygon(true);
            mode_manager.currently_baking = true;
        }
    }

    public void rebake() {
        region.BakeNavigationPolygon();
    }

    public void free_to_rebake() {
        mode_manager.currently_baking = false;
        placed = true;
        if(mode_manager.buildings_to_bake.Count > 0) {
            mode_manager.buildings_to_bake[0].rebake_add_building();
            mode_manager.buildings_to_bake[0].rebake();
        }
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) {
        if (@event.IsActionPressed("LeftClick") && mode_manager.current_mode is not BuildMode) {
            UI ui = GetNode<UI>("/root/Game/UI");
            ui.instantiate_window("TowerMenu", this);
        }
    }
}
