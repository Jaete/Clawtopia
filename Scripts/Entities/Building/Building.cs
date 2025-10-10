using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using ClawtopiaCs.Scripts.Systems.Tooling;
using Godot;
using Godot.Collections;
using static BuildingData;
using ClawtopiaCs.Scripts.Entities;

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

    [Export]
    public bool IsPreSpawned
    {
        get => _isPreSpawned;
        set
        {
            _isPreSpawned = value;
            if (Engine.IsEditorHint() && IsNodeReady())
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
            if (Engine.IsEditorHint() && IsNodeReady())
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
            if (_type == value) return;
            _type = value;
            if (Engine.IsEditorHint() && IsNodeReady())
            {
                if (string.IsNullOrEmpty(value)) 
                {
                    Data = null;
                    Reset(this);
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
            if (_buildingData is null && IsNodeReady())
            {
                Reset(this);
                return;
            }

            if (Engine.IsEditorHint() && IsNodeReady()) 
            {
                BuildingEditor.ReloadBuilding(this); 
            }
        }
    }

    public bool IsInitializing = true;

    public bool IsBuildingInFront;
    public bool IsBuilt;
    public bool Placed;

    // HP BAR
    private MapHpBar _hpBar;

    // TIMER PARA TICK DE TEMPO DE CONSTRUCAO
    public Timer BuildTickTimer;
    public Array<Ally> CurrentBuilders = new();
    public int MaxProgress = 50;
    public int Progress;
    public float TickTime = 1.0f;

    public string ResourceType;
    public int SelfIndex;

    public override void _EnterTree()
    {
        IsInitializing = true;
    }

    public override void _Ready()
    {
        CallDeferred(MethodName.Initialize);

        _hpBar = GetNode<MapHpBar>("MapHpBar");
        _hpBar.SetBuilding(this);
    }

    public void Initialize()
    {
        IsBuilt = IsPreSpawned;

        if (!Engine.IsEditorHint())
        {
            CallDeferred(MethodName.ConnectSignals);
        } 

        CallDeferred(MethodName.LoadData);

        if (IsPreSpawned) { CallDeferred(MethodName.PlaceBuilding, this); }
        IsInitializing = false;
    }

    protected void LoadData()
    {
        if (Type == null)
        {
            GD.PushError("Building Type is not set. Please assign it in the editor.");
            return;
        }
        Data ??= BuildingEditor.LoadBuildingData(Type, Buildings);
        Data.Initialize(this);
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
        LevelManager.Singleton.CollectorBuildings.Add(this);
    }

    public void RemoveSelfFromList()
    {
        LevelManager.Singleton.CollectorBuildings.Remove(this);
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
        var RefundResource = new Dictionary<Collectable, int>();
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
        if (Engine.IsEditorHint()) return; 

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