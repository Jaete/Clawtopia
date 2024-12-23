using System;
using Godot;
using Godot.Collections;

public partial class BuildingData : Resource
{
    public const string PROP_ISBUILDING = "IsBuilding";
    [Export] public int CatnipCost;
    [Export] public ConcavePolygonShape2D GridShape;
    [Export] public int Hp;
    [Export] public Vector2 InteractionOffset;
    [Export] public ConcavePolygonShape2D InteractionShape;
    [Export] public bool IsPreSpawned;
    public int Level;
    [Export] public int MaxProgress;
    public String Name;
    [Export] public bool NeedsRegion = false;
    [Export] public ConcavePolygonShape2D ObstacleShape;

    [Export] public Vector2 Offset;
    [Export] public Rect2 RegionRect;

    public Dictionary<string, int> ResourceCosts = new();

    [Export(PropertyHint.Enum, Constants.RESOURCE_LIST)]
    public string ResourceType;

    [Export] public int SalmonCost;
    [Export] public int SandCost;
    [Export] public Vector2 Scale = new(1, 1);
    [Export] public Texture2D SpriteTexture;

    [Export(PropertyHint.Enum, Constants.TOWER_LIST)]
    public string TowerType;

    [Export(PropertyHint.Enum, Constants.BUILDING_LIST)]
    public string Type;

    public void Initialize()
    {
        Level = 1;
        ResourceCosts[Constants.SALMON] = SalmonCost;
        ResourceCosts[Constants.CATNIP] = CatnipCost;
        ResourceCosts[Constants.SAND] = SandCost;
    }
}