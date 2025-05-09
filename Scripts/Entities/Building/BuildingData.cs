using System;
using ClawtopiaCs.Scripts.Entities.Building;
using Godot;
using Godot.Collections;

public partial class BuildingData : Resource
{
    [ExportGroup("Audio Settings")]
    [Export] public AudioStream PlaceBuildingSound = GD.Load<AudioStream>("res://Assets/Audio/SFX/Building/plot.ogg");
    [Export] public Array<AudioStream> DestroyBuildingSounds = new()
    {
        GD.Load<AudioStream>("res://Assets/Audio/SFX/Building/destroy-001.ogg"),
        GD.Load<AudioStream>("res://Assets/Audio/SFX/Building/destroy-002.ogg"),
        GD.Load<AudioStream>("res://Assets/Audio/SFX/Building/destroy-003.ogg"),
        GD.Load<AudioStream>("res://Assets/Audio/SFX/Building/destroy-004.ogg"),
        GD.Load<AudioStream>("res://Assets/Audio/SFX/Building/destroy-005.ogg"),
    };
    [Export] public AudioStream CancelSound = GD.Load<AudioStream>("res://Assets/Audio/UI/ui-click-cancel.ogg");

    [ExportGroup("Type Settings")]
    [Export(PropertyHint.Enum, Constants.BUILDING_LIST)]
    public string Type;
    [Export(PropertyHint.Enum, Constants.RESOURCE_LIST)]
    public string ResourceType;
    [Export(PropertyHint.Enum, Constants.TOWER_LIST)]
    public string TowerType;

    [ExportGroup("Mechanic Settings")]
    [Export] public int CatnipCost;
    [Export] public int SalmonCost;
    [Export] public int SandCost;
    [Export] public int Hp;
    [Export] public int MaxProgress;

    [ExportGroup("Structure")]
    [Export] public BuildingStructure Structure;
    [Export] public Vector2 Scale = new(1, 1);
    [Export] public Vector2 Offset;


    public const string PROP_ISBUILDING = "IsBuilding";
    public int Level;
    private string type;
    public String Name;
    public Dictionary<string, int> ResourceCosts = new();

    public void Initialize()
    {
        Level = 1;
        ResourceCosts[Constants.SALMON] = SalmonCost;
        ResourceCosts[Constants.CATNIP] = CatnipCost;
        ResourceCosts[Constants.SAND] = SandCost;
    }
}