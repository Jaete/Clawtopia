using Godot;
using System;

public partial class BuildingData : Resource
{
    [Export] public Vector2 OFFSET;
    [Export] public Vector2 SCALE;
    [Export] public Vector2 INTERACTION_OFFSET;
    [Export] public ConcavePolygonShape2D OBSTACLE_SHAPE;
    [Export] public ConcavePolygonShape2D INTERACTION_SHAPE;
    [Export] public ConcavePolygonShape2D GRID_SHAPE;
    [Export] public Texture2D SPRITE_TEXTURE;    
    [Export] public String TYPE;
    [Export] public String NAME;
    [Export] public int HP;
    [Export] public bool NEEDS_REGION = false;
    [Export] public Rect2 REGION_RECT;
    
    public int level;

    public void initialize() {
        level = 1;
    }
}
