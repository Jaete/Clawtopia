using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using ClawtopiaCs.Scripts.Systems.Tooling;
using Godot;
using Godot.Collections;
using static BuildingData;
using ClawtopiaCs.Scripts.Entities;
using System.Linq;

[GlobalClass, Tool]
public partial class Building : Area2D
{
    [Signal]
    public delegate void AboutToInteractEventHandler(Building self);

    [Signal]
    public delegate void RemovedInteractionEventHandler(Building self);

    [Signal]
    public delegate void DestroyedEventHandler ();

    [ExportGroup("Building Settings")]
    [Export]
    public BuildingList Buildings;

    [ExportGroup("Building states")]
    [Export] public bool IsPreSpawned
    {
        get => _isPreSpawned;
        set
        {
            _isPreSpawned = value;
            if (Engine.IsEditorHint()) 
            {
                BuildingEditor.ReloadBuilding(this); 
            }
        }
    }

    [Export] public bool IsRotated
    {
        get => _isRotated;
        set
        {
            _isRotated = value;
            if (Engine.IsEditorHint())
            {
                BuildingEditor.ReloadBuilding(this);
            }
        }
    }

    private bool _isPreSpawned = false;
    private bool _isRotated = false;
    private BuildingData _buildingData;
    private string _type = string.Empty;

    [ExportGroup("Node Refs")]
    [Export] public CollisionPolygon2D BodyShape;
    [Export] public CollisionPolygon2D GridShape;
    [Export] public CollisionPolygon2D InteractionShape;
    [Export] public Sprite2D Sprite;
    [Export] public StaticBody2D StaticBody;
    [Export] public AudioStreamPlayer Sounds;

    [ExportGroup("Structure")]
    [Export(PropertyHint.Enum)]
    public string Type
    {
        get => _type ?? string.Empty;
        set
        {
            if (!Engine.IsEditorHint() || _type == value) return;
            _type = value;
            if (Engine.IsEditorHint())
            {
                if (string.IsNullOrEmpty(value)) 
                {
                    Data = null;
                    Reset(this);
                    GD.Print("Jhonson?");
                    return;
                }
                if (Data != null && Data.Name == value) return;
                Data = BuildingEditor.LoadBuildingData(value, Buildings);
                BuildingEditor.ReloadBuilding(this);
            }
        }
    }
    [Export] public BuildingData Data {
        get => _buildingData;
        set
        {
            if (_buildingData == value) return;
            _buildingData = value;
            if (_buildingData is null)
            {
                Reset(this);
                GD.Print("Jhonson?");
                return;
            }

            if (Engine.IsEditorHint()) 
            {
                BuildingEditor.ReloadBuilding(this); 
            }
        }
    }

    public bool IsInitializing { get; internal set; }

    public bool IsBuildingInFront;
    public bool IsBuilt;
    public bool Placed;

    // TIMER PARA TICK DE TEMPO DE CONSTRUCAO
    public Timer BuildTickTimer;
    public Array<Ally> CurrentBuilders = new();
    public int MaxProgress = 50;
    public int Progress;
    public float TickTime = 1.0f;

    public string ResourceType;
    public int SelfIndex;

    public override void _Ready()
    {
        if (!Engine.IsEditorHint() && !IsInitializing)
        {
            CallDeferred(MethodName.Initialize);
        }
    }

    public void Initialize()
    {
        if (IsInitializing) return; 

        IsBuilt = IsPreSpawned;

        if (Engine.IsEditorHint())
        {
            if (Type == null)
            {
                GD.PushError("Building Type is not set. Please assign it in the editor.");
                return;
            }
            Data ??= Buildings?.List.FirstOrDefault(b => b.Name == Type) ?? null;
            Data.CallDeferred(BuildingData.MethodName.Initialize, this);
            return;
        }
        
        ConnectSignals();
        Data.Initialize(this);

        if (IsPreSpawned) { CallDeferred(MethodName.PlaceBuilding, this); }
    }

    public void ConnectSignals()
    {   
        AboutToInteract += SimulationMode.Singleton.AboutToInteractWithBuilding;
        RemovedInteraction += SimulationMode.Singleton.InteractionWithBuildingRemoved;
        BuildMode.Singleton.ConstructionStarted += ConstructionStarted;
        BuildTickTimer = new Timer();
        BuildTickTimer.OneShot = true;
        BuildTickTimer.Timeout += ConstructionTimeElapsed;
        Destroyed += OnDestroyed;
        AddChild(BuildTickTimer);
    }

    public void AddSelfOnList()
    {
        switch (Data.Resource)
        {
            case BuildingData.ResourceType.Salmon:
                ResourceType = Constants.SALMON;
                LevelManager.Singleton.SalmonBuildings.Add(this);
                break;
            case BuildingData.ResourceType.Catnip:
                ResourceType = Constants.CATNIP;
                LevelManager.Singleton.CatnipBuildings.Add(this);
                break;
            case BuildingData.ResourceType.Sand:
                ResourceType = Constants.SAND;
                LevelManager.Singleton.SandBuildings.Add(this);
                break;
        }
    }

    public void RemoveSelfFromList()
    {
        switch (Data.Resource)
        {
            case BuildingData.ResourceType.Salmon:
                LevelManager.Singleton.SalmonBuildings.Remove(this);
                break;
            case BuildingData.ResourceType.Catnip:
                LevelManager.Singleton.CatnipBuildings.Remove(this);
                break;
            case BuildingData.ResourceType.Sand:
                LevelManager.Singleton.SandBuildings.Remove(this);
                break;
        }
    }

    public async void ConstructionStarted(Building building)
    {
        Progress = 0;
        BuildTickTimer.Start(TickTime);
        Sounds.Stream = Data.PlaceBuildingSound;
        Sounds.Play();
        await ToSignal(Sounds, AudioStreamPlayer2D.SignalName.Finished);
    }

    public async void OnDestroyed()
    {
        var RefundResource = new Dictionary<ResourceType, int>();
        foreach (var resource in Data.ResourceCosts)
        {
            RefundResource[resource.Key] = resource.Value / 2;
        }
        LevelManager.Singleton.EmitSignal(LevelManager.SignalName.ResourceDelivered, RefundResource);

        Sounds.Stream = Data.DestroyBuildingSounds[GD.RandRange(0, Data.DestroyBuildingSounds.Count - 1)];
        Sounds.Play();
        await ToSignal(Sounds, AudioStreamPlayer2D.SignalName.Finished);
        QueueFree();
    }

    public void ConstructionTimeElapsed()
    {
        var nextProgress = Progress + CurrentBuilders.Count;

        if (nextProgress < MaxProgress)
        {
            Progress = nextProgress;
            BuildTickTimer.Start(TickTime);
            return;
        }

        BuildMode.Singleton.EmitSignal(GameMode.SignalName.BuildCompleted, this);
        IsBuilt = true;
        BuildTickTimer.Timeout -= ConstructionTimeElapsed;
        BuildTickTimer.Stop();
    }

    public override void _MouseEnter()
    {
        if (ModeManager.Singleton.CurrentMode is not SimulationMode)
        {
            return;
        }

        if (SimulationMode.Singleton.BuildingsToInteract.Count == 0 || SimulationMode.Singleton.BuildingsToInteract[0] != this)
        {
            EmitSignal(SignalName.AboutToInteract, this);
        }
    }


    public override void _MouseExit()
    {
        if (ModeManager.Singleton.CurrentMode is not SimulationMode)
        {
            return;
        }

        EmitSignal(SignalName.RemovedInteraction, this);
    }

    public override void _ExitTree()
    {
        if (Engine.IsEditorHint()) return;
        RemoveSelfFromList();
    }


    public static void PlaceBuilding(Building building)
    {      
        building.IsBuilt = true;
        Modulation.AssignState(building, InteractionStates.FINISHED);
        building.Data.SetSpriteTexture(building);
        building.AddSelfOnList();
        building.CurrentBuilders.Clear();
    }

    public static void RotateBuildingPreview(Building building)
    {
        building.IsRotated = !building.IsRotated;
        building.Data.SetSpriteTexture(building);
        building.Data.SetCollisionPolygon(building);
        building.Data.SetInteractionPolygon(building);
        building.Data.SetGridPolygon(building);
    }

    public override void _ValidateProperty(Dictionary property)
    {
        if (!Engine.IsEditorHint())
        {
            base._ValidateProperty(property);
            return;
        };

        if (property["name"].AsStringName() == PropertyName.Type)
        {
            var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
            property["hint_string"] = string.Join(",", BuildingLoader.GetBuildingNames(Buildings));
        }

        base._ValidateProperty(property);
    }
}