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
        Initialize();
    }

    public void Initialize() {
        mode_manager = GetNode<ModeManager>("/root/Game/ModeManager");
        building_mode = GetNode<BuildMode>("/root/Game/ModeManager/BuildMode");
        region = GetNode<NavigationRegion2D>("../Navigation");
        static_body = GetNode<StaticBody2D>("NavigationBody");
        data.initialize();
        body_shape.Polygon = data.OBSTACLE_SHAPE.Segments;
        interaction_shape.Shape = data.INTERACTION_SHAPE;
        grid_shape.Polygon = data.GRID_SHAPE.Segments;
        sprite.Texture = data.SPRITE_TEXTURE;
        sprite.Offset = data.OFFSET;
        sprite.Scale = data.SCALE;
        interaction_shape.Position = data.INTERACTION_OFFSET;
        if(data.TYPE.Equals("GreatCommune")) {
            Name = data.TYPE;
        } else {
            Name = data.NAME + "_" + data.TYPE + "_" + self_index;
        }
        region.BakeFinished += When_free_to_rebake;
    }

    public void Set_rebake() {
        static_body.Name = "Obstacle_Region_" + data.TYPE + "_" + self_index;
        static_body.Reparent(region);
        mode_manager.currently_baking = true;
    }

    public void Rebake_add_building() {
        if (is_pre_spawned) {
            Set_rebake();
            return;
        }
        if (mode_manager.current_mode is BuildMode) {
            Set_rebake();
            return;
        }
    }

    public void Rebake_remove_building() {
        if (!mode_manager.currently_baking) {
            StaticBody2D obstacle = region.GetNode<StaticBody2D>("Obstacle_Region_" + data.TYPE + "_" + self_index);
            obstacle.Reparent(this);
            region.BakeNavigationPolygon(true);
            mode_manager.currently_baking = true;
        }
    }

    public void Rebake() {
        region.BakeNavigationPolygon();
    }

    public void When_free_to_rebake() {
        mode_manager.currently_baking = false;
        placed = true;
        if(mode_manager.buildings_to_bake.Count > 0) {
            mode_manager.buildings_to_bake[0].Rebake_add_building();
            mode_manager.buildings_to_bake[0].Rebake();
        }
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) {
        if (@event.IsActionPressed("LeftClick") && mode_manager.current_mode is not BuildMode) {
            UI ui = (UI)GetNode("/root/Game/UI");
            ui.Instantiate_window("TowerMenu", this);
        }
    }
}
