using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;
using Godot.Collections;

public partial class Building : Area2D
{
    [Signal]
    public delegate void AboutToInteractEventHandler(Building self);

    [Signal]
    public delegate void RemovedInteractionEventHandler(Building self);

    [Export] public CollisionPolygon2D BodyShape;
    public BuildMode BuildingMode;

    // TIMER PARA TICK DE TEMPO DE CONSTRUCAO
    public Timer BuildTickTimer;
    public Array<Ally> CurrentBuilders = new();
    [Export] public BuildingData Data;
    public Color ErrorColor = new Color("ba000079");
    [Export] public CollisionPolygon2D GridShape;
    public Color HoverColor = new Color(1.3f, 1.3f, 1.3f);
    [Export] public CollisionPolygon2D InteractionShape;
    public bool IsBuildingInFront;
    public bool IsBuilt;

    [Export] public bool IsPreSpawned = false;
    public LevelManager LevelManager;
    public int MaxProgress = 50;
    public ModeManager ModeManager;

    public Color OkColor = new Color("2eff3f81");
    public bool Placed;
    public int Progress;

    public NavigationRegion2D Region;
    public Color RegularColor = new Color(1, 1, 1);

    public string ResourceType;

    public int SelfIndex;
    public SimulationMode SimulationModeRef;
    [Export] public Sprite2D Sprite;
    [Export] public StaticBody2D StaticBody;

    // TEMPO EM SEGUNDOS POR TICK
    public float TickTime = 1.0f;

    public override void _Ready()
    {
        Initialize();
    }

    public void Initialize()
    {
        IsPreSpawned = Data.IsPreSpawned;
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
        BuildingMode = GetNode<BuildMode>("/root/Game/ModeManager/BuildMode");
        Region = GetNode<NavigationRegion2D>("../Navigation");
        StaticBody = GetNode<StaticBody2D>("NavigationBody");
        SimulationModeRef = GetNode<SimulationMode>("/root/Game/ModeManager/SimulationMode");
        LevelManager = GetNode<LevelManager>("/root/Game/LevelManager");
        Data.Initialize();
        BodyShape.Polygon = Data.ObstacleShape.Segments;
        InteractionShape.Polygon = Data.InteractionShape.Segments;
        InteractionShape.Position = Data.InteractionOffset;
        GridShape.Polygon = Data.GridShape.Segments;
        Sprite.Texture = Data.SpriteTexture;
        Sprite.Offset = Data.Offset;
        Sprite.Scale = Data.Scale;
        Sprite.RegionEnabled = false;
        if (Data.NeedsRegion) {
            Sprite.RegionEnabled = true;
            Sprite.RegionRect = Data.RegionRect;
        }

        Name = Data.Name + "_" + Data.Type + "_" + SelfIndex;
        if (Data.Type.Equals(Constants.COMMUNE)) {
            Name = Constants.COMMUNE_EXTERNAL_NAME;
        }

        if (Data.Type == Constants.HOUSE) {
            Name = Constants.HOUSE_EXTERNAL_NAME;
        }

        if (Data.Type == Constants.TOWER) {
            switch (Data.TowerType) {
                case Constants.FIGHTERS:
                    Name = Constants.FIGHTERS_TOWER_EXTERNAL_NAME;
                    break;
                /*todo implementar o resto*/
            }
        }

        if (Data.Type == Constants.RESOURCE) {
            switch (Data.ResourceType) {
                case Constants.SALMON:
                    Name = Constants.FISHERMAN_HOUSE_EXTERNAL_NAME;
                    break;
                case Constants.CATNIP:
                    /*TODO IMPLEMENTAR*/
                    break;
                case Constants.SAND:
                    /*TODO IMPLEMENTAR*/
                    break;
            }
        }

        MaxProgress = OS.IsDebugBuild() ? 3 : Data.MaxProgress;
        Region.BakeFinished += FreeToRebake;
        AboutToInteract += SimulationModeRef.AboutToInteractWithBuilding;
        RemovedInteraction += SimulationModeRef.InteractionWithBuildingRemoved;
        BuildingMode.ConstructionStarted += ConstructionStarted;
        BuildTickTimer = new Timer();
        BuildTickTimer.OneShot = true;
        BuildTickTimer.Timeout += ConstructionTimeElapsed;
        AddChild(BuildTickTimer);
        CallDeferred("AddSelfOnList");

        if (!IsPreSpawned) {
            return;
        }

        IsBuilt = true;
    }

    public void AddSelfOnList()
    {
        switch (Data.ResourceType) {
            case Constants.SALMON:
                ResourceType = Constants.SALMON;
                LevelManager.SalmonBuildings.Add(this);
                break;
            case Constants.CATNIP:
                ResourceType = Constants.CATNIP;
                LevelManager.CatnipBuildings.Add(this);
                break;
            case Constants.SAND:
                ResourceType = Constants.SAND;
                LevelManager.SandBuildings.Add(this);
                break;
        }
    }

    public void RemoveSelfFromList()
    {
        switch (Data.ResourceType) {
            case Constants.SALMON:
                LevelManager.SalmonBuildings.Remove(this);
                break;
            case Constants.CATNIP:
                LevelManager.CatnipBuildings.Remove(this);
                break;
            case Constants.SAND:
                LevelManager.SandBuildings.Remove(this);
                break;
        }
    }

    public void SetRebake()
    {
        StaticBody.Name = "Obstacle_Region_" + Data.Type + "_" + SelfIndex;
        StaticBody.Reparent(Region);
        ModeManager.CurrentlyBaking = true;
    }

    public void RebakeAddBuilding(bool initialize = false)
    {
        if (ModeManager.CurrentMode is not BuildMode && !initialize) {
            return;
        }

        SetRebake();
    }

    public void RebakeRemoveBuilding()
    {
        if (!ModeManager.CurrentlyBaking) {
            StaticBody2D obstacle = Region.GetNode<StaticBody2D>("Obstacle_Region_" + Data.Type + "_" + SelfIndex);
            obstacle.Reparent(this);
            Rebake();
            ModeManager.CurrentlyBaking = true;
        }
    }

    public void Rebake()
    {
        Region.BakeNavigationPolygon();
    }

    public void FreeToRebake()
    {
        ModeManager.CurrentlyBaking = false;
        Placed = true;
        if (ModeManager.BuildingsToBake.Count > 0) {
            ModeManager.BuildingsToBake[0].RebakeAddBuilding();
            ModeManager.BuildingsToBake[0].Rebake();
            ModeManager.BuildingsToBake.RemoveAt(0);
        }
    }

    public void ConstructionStarted(Building building)
    {
        Progress = 0;
        BuildTickTimer.Start(TickTime);
    }

    public void ConstructionTimeElapsed()
    {
        var nextProgress = Progress + CurrentBuilders.Count;
        if (nextProgress < MaxProgress) {
            Progress = nextProgress;
            BuildTickTimer.Start(TickTime);
            return;
        }

        BuildingMode.EmitSignal("BuildCompleted", this);
        IsBuilt = true;
        BuildTickTimer.Timeout -= ConstructionTimeElapsed;
    }

    public override void _MouseEnter()
    {
        if (ModeManager.CurrentMode is not SimulationMode) {
            return;
        }

        if (SimulationModeRef.BuildingsToInteract.Count == 0 || SimulationModeRef.BuildingsToInteract[0] != this) {
            EmitSignal("AboutToInteract", this);
        }
    }


    public override void _MouseExit()
    {
        if (ModeManager.CurrentMode is not SimulationMode) {
            return;
        }

        EmitSignal("RemovedInteraction", this);
    }

    public override void _ExitTree()
    {
        RemoveSelfFromList();
    }

    public static void ModulateBuilding(Building building, BuildingInteractionStates state)
    {
        switch (state) {
            case BuildingInteractionStates.HOVER:
                building.Modulate = building.IsBuilt ? building.HoverColor : building.OkColor;
                break;
            case BuildingInteractionStates.UNHOVER:
                building.Modulate = building.IsBuilt ? building.RegularColor : building.OkColor;
                break;
            case BuildingInteractionStates.BUILDING_ERROR:
                building.Modulate = building.ErrorColor;
                break;
            case BuildingInteractionStates.BUILDING_OK:
                building.Modulate = building.OkColor;
                break;
            case BuildingInteractionStates.BUILD_FINISHED:
                building.Modulate = building.RegularColor;
                break;
        }
    }
}