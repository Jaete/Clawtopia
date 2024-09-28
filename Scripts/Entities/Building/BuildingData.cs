using Godot;
using System;

public partial class BuildingData : Resource
{
    [Export] public Vector2 Offset;
    [Export] public Vector2 Scale;
    [Export] public Vector2 InteractionOffset;
    [Export] public ConcavePolygonShape2D ObstacleShape;
    [Export] public ConcavePolygonShape2D InteractionShape;
    [Export] public ConcavePolygonShape2D GridShape;
    [Export] public Texture2D SpriteTexture;
    [Export(PropertyHint.Enum, Constants.BUILDING_LIST)] public string Type;
    [Export(PropertyHint.Enum, Constants.TOWER_LIST)] public string TowerType;
    [Export(PropertyHint.Enum, Constants.RESOURCE_LIST)] public string ResourceType;
    [Export] public int Hp;
    [Export] public bool NeedsRegion = false;
    [Export] public Rect2 RegionRect;
    [Export] public int MaxProgress;
    public int Level;
    public String Name;
    [Export] public int SalmonCost = 100;
    [Export] public int CatnipCost = 100;
    [Export] public int SandCost = 100;

    public void Initialize() {
        Level = 1;
    }
}
