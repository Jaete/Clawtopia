using Godot;
using System;
using System.Reflection.Metadata;

public partial class BuildingData : Resource
{
    [Export] public Vector2 OFFSET;
    [Export] public Vector2 SCALE;
    [Export] public Vector2 INTERACTION_OFFSET;
    [Export] public ConcavePolygonShape2D OBSTACLE_SHAPE;
    [Export] public ConcavePolygonShape2D INTERACTION_SHAPE;
    [Export] public ConcavePolygonShape2D GRID_SHAPE;
    [Export] public Texture2D SPRITE_TEXTURE;    
    [Export(PropertyHint.Enum, Constants.BUILDING_LIST)] public string TYPE;
    [Export(PropertyHint.Enum, Constants.TOWER_LIST)]public string TOWER_TYPE;
    [Export(PropertyHint.Enum, Constants.RESOURCE_LIST)] public string RESOURCE_TYPE;
    [Export] public int HP;
    [Export] public bool NEEDS_REGION = false;
    [Export] public Rect2 REGION_RECT;
    [Export] public int max_progress;
    public int level;
    public String NAME;

    public void initialize() {
        level = 1;
    }
}
