using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Systems;
using Godot;
using Godot.Collections;
using ClawtopiaCs.Scripts.Systems.Tooling;

[GlobalClass, Tool]
public partial class BuildingData : Resource
{

    public enum Type
    {
        None = -1,
        Commune = 0,
        Resource,
        Tower,
        House
    }

    public enum ResourceType
    {
        None = -1,
        Salmon = 0,
        Catnip,
        Sand
    }

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

    [ExportGroup("UI Settings")]
    [Export(PropertyHint.File)] public string UIMenu = null;

    [ExportGroup("Type Settings")]
    [Export(PropertyHint.Enum)]
    public Type BuildingType;
    [Export(PropertyHint.Enum, Constants.RESOURCE_LIST)]
    public Collectable Resource;
    [Export(PropertyHint.Enum, Constants.TOWER_LIST)]
    public string TowerType;
    [Export(PropertyHint.Enum)]
    public string Name;

    [ExportGroup("Mechanic Settings")]
    [Export] public Dictionary<Collectable, int> ResourceCosts = new();
    [Export] public int Hp;
    [Export] public int MaxProgress;

    [ExportGroup("Structure")]
    [Export] public BuildingStructure Structure = null;
    [Export] public Vector2 Scale = new(1, 1);
    [Export] public Vector2 Offset { get; set; }

    [Export]
    public Vector2 RotatedOffset { get; set; }

    private Building _building;
    private Vector2 _offset = Vector2.Zero;
    private Vector2 _rotatedOffset = Vector2.Zero;
    public int Level;

   

    public void Initialize(Building building)
    {
        if (!ValidateBuilding(building, MethodName.Initialize)) return;
        building.IsInitializing = true;
        _building = building;

        SetSpriteTexture(building);
        SetCollisionPolygon(building);
        SetGridPolygon(building);
        SetInteractionPolygon(building);

        if (Engine.IsEditorHint()) {
            building.IsInitializing = false;
            return;
        }

        Level = 1;
        if (BuildingType == Type.Commune)
        {
            LevelManager.Singleton.Purrlament = building;
        }
        building.Name = Name;
        building.MaxProgress = OS.IsDebugBuild() ? 3 : MaxProgress;
        building.IsInitializing = false;
    }
    public static void Reset(Building building)
    {
        if (!ValidateBuilding(building, MethodName.Reset, true)) return;
        building.Sprite.Texture = null;
        building.BodyShape.Polygon = null;
        building.GridShape.Polygon = null;
        building.InteractionShape.Polygon = null;
        building.Sprite.Offset = Vector2.Zero;

        if (Engine.IsEditorHint())
        {
            BuildingEditor.ReloadBuilding(building);
        }
    }
    
    public void SetSpriteTexture(Building building)
    {
        if (building.IsBuilt)
        {
            if (building.IsRotated)
            {
                building.Sprite.Texture = Structure.RotatedPlacedTexture;
                building.Sprite.Offset = Structure.RotatedPlacedOffset;
            }
            else
            {
                building.Sprite.Texture = Structure.PlacedTexture;
                building.Sprite.Offset = Structure.PlacedOffset;
            }
        }
        else
        {
            if (building.IsRotated)
            {
                building.Sprite.Texture = Structure.RotatedPreviewTexture;
                building.Sprite.Offset = RotatedOffset;
            }
            else
            {
                building.Sprite.Texture = Structure.PreviewTexture;
                building.Sprite.Offset = Offset;
            }
        }
    }

    public void SetInteractionPolygon(Building building)
    {
        if (Structure.RotatedInteraction is null || Structure.Interaction is null)
        {
            GD.PushWarning("Structure interaction segments are not set. If adding a new building, this is intended behaviour and you can ignore this warning.");
            return;
        }

        building.InteractionShape.Polygon = building.IsRotated
            ? Structure.RotatedInteraction.Segments
            : Structure.Interaction.Segments;
    }


    public void SetCollisionPolygon(Building building)
    {
        if (Structure.RotatedCollision is null || Structure.Collision is null)
        {
            GD.PushWarning("Structure collision segments are not set. If adding a new building, this is intended behaviour and you can ignore this warning.");
            return;
        }
        building.BodyShape.Polygon = building.IsRotated 
            ? Structure.RotatedCollision.Segments 
            : Structure.Collision.Segments;
    }

    public void SetGridPolygon(Building building)
    {
        if (Structure.RotatedGridArea is null || Structure.GridArea is null)
        {
            GD.PushWarning("Structure grid segments are not set. If adding a new building, this is intended behaviour and you can ignore this warning.");
            return;
        }

        building.GridShape.Polygon = building.IsRotated
            ? Structure.RotatedGridArea.Segments
            : Structure.GridArea.Segments;
    }

    public static bool ValidateBuilding(Building building, StringName source, bool shouldBeNull = false, bool silent = false)
    {
        if (building != null && building.Data != null && building.Data.Structure != null)
        {
            return true;
        }
        if (shouldBeNull && building != null)
        {
            return true;
        }
        if (silent)
        {
            return false;
        }

        GD.PushWarning("\nNode refs, Data or Structure are missing. Probably Godot resetted some exported variables. \nSource: ", building?.Name ?? "Null building", "\nMethodName: ", source);
        return false;
    }

    public override void _ValidateProperty(Dictionary property)
    {
        if (!Engine.IsEditorHint())
        {
            base._ValidateProperty(property);
            return;
        };

        if (!ValidateBuilding(_building, MethodName._ValidateProperty, silent: true))
        {
            return;
        }

        if (property["name"].AsStringName() == PropertyName.Name)
        {
            var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
            property["hint_string"] = string.Join(",", BuildingLoader.GetBuildingNames(_building.Buildings));
        }

        base._ValidateProperty(property);
    }
}