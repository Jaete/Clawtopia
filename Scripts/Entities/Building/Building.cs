using Godot;
using System;
using System.IO;

public partial class Building : Area2D {
    
    public NavigationRegion2D region;
    public Director director;

    
    public int self_index;
    public bool placed;

    [Export] public bool is_pre_spawned = false;
    [Export] public BuildingData data;
    [Export] public StaticBody2D static_body;
    [Export] public CollisionPolygon2D body_shape;
    [Export] public CollisionShape2D interaction_shape;
    [Export] public Sprite2D sprite;

    public override void _Ready() {
        initialize();
        CallDeferred("rebake_add_building");
    }

    public void initialize() {
        director = GetNode<Director>("/root/Game/Director");
        region = (NavigationRegion2D)GetNode("../Navigation");
        static_body = GetNode<StaticBody2D>("NavigationBody");
        data.initialize();
        body_shape.Polygon = data.obstacle_shape.Segments;
        interaction_shape.Shape = data.interaction_shape;
        sprite.Texture = data.sprite_texture;
        sprite.Offset = data.sprite_offset;
        sprite.Scale = data.sprite_size;
        interaction_shape.Position = data.interaction_offset;
        if(data.type == "GreatCommune") {
            Name = data.type;
        } else {
            Name = data.name + "_" + data.type + "_" + self_index;
        }
        region.BakeFinished += free_to_rebake;
    }

    public void set_rebake() {
        static_body.Name = "Obstacle_Region_" + data.type + "_" + self_index;
        static_body.Reparent(region);
        director.currently_baking = true;
        region.BakeNavigationPolygon();
    }

    public void rebake_add_building() {
        if (is_pre_spawned) {
            GD.Print("Trying to bake -> ", this.Name);
            if (!director.currently_baking) {
                set_rebake();
            } else {
                CallDeferred("set_rebake");
            }
            return;
        }
        if (director.current_mode is BuildMode) {
            set_rebake();
            return;
        }
    }

    public void rebake_remove_building() {
        if (!director.currently_baking) {
            StaticBody2D obstacle = (StaticBody2D)region.GetNode("Obstacle_Region_" + this.self_index);
            obstacle.Reparent(this);
            region.BakeNavigationPolygon(true);
            director.currently_baking = true;
        }
    }

    public void free_to_rebake() {
        GD.Print("Bake finished.");
        director.currently_baking = false;
        placed = true;
        if(director.buildings_to_bake.Count > 0) {
            director.buildings_to_bake.RemoveAt(0);
            CallDeferred("rebake_add_building");
        }
    }
}
