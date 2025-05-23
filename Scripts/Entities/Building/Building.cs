using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;
using Godot.Collections;
using static Godot.WebSocketPeer;

public partial class Building : Area2D
{
    [Signal]
    public delegate void AboutToInteractEventHandler(Building self);

    [Signal]
    public delegate void RemovedInteractionEventHandler(Building self);

    [Signal]
    public delegate void DestroyedEventHandler ();

    private int _progress;

    private BuildingData _buildingData;

    [ExportGroup("Node Refs")]
    [Export] public CollisionPolygon2D BodyShape;
    [Export] public CollisionPolygon2D GridShape;
    [Export] public CollisionPolygon2D InteractionShape;
    [Export] public Sprite2D Sprite;
    [Export] public StaticBody2D StaticBody;
    [Export] public AudioStreamPlayer Sounds;

    [ExportGroup("Structure")]
    [Export] public BuildingData Data;
    public ProgressStructure Structure;

    [Export] public bool IsPreSpawned = false;

    public Color ErrorColor = new Color("ba000079");
    public Color HoverColor = new Color(1.3f, 1.3f, 1.3f);
    public Color OkColor = new Color("2eff3f81");
    public Color RegularColor = new Color(1, 1, 1);

    //( Internal Logic )
    public string ResourceType;
    public int SelfIndex;
    public bool IsBuildingInFront;
    public bool IsBuilt;
    public bool Placed;
    public bool IsRotated;

    //( Build time logic )
    public Timer BuildTickTimer;
    public Array<Ally> CurrentBuilders = new();
    public int MaxProgress = 50;
    public int Progress {
        get => _progress;
        set {
            _progress = value;
            ChangeSpriteOnBreakpoint();
        }
    }


    public float TickTime = 1.0f;

    public override void _Ready()
    {
        CallDeferred(MethodName.Initialize);
    }

    public void Initialize()
    { 
        StaticBody = GetNode<StaticBody2D>("NavigationBody");
        Data.Initialize();
        Sprite.Texture = Data.Structure.PreviewTexture;
        BodyShape.Polygon = Data.Structure.Collision.Segments;
        InteractionShape.Polygon = Data.Structure.Interaction.Segments;
        GridShape.Polygon = Data.Structure.Collision.Segments;

        Name = Data.Name + "_" + Data.Type + "_" + SelfIndex;

        if (Data.Type.Equals(Constants.COMMUNE))
        {
            Name = Constants.COMMUNE_EXTERNAL_NAME;
            LevelManager.Singleton.Purrlament = this;
        }

        if (Data.Type == Constants.HOUSE)
        {
            Name = Constants.HOUSE_EXTERNAL_NAME;
        }

        if (Data.Type == Constants.TOWER)
        {
            switch (Data.TowerType)
            {
                case Constants.FIGHTERS:
                    Name = Constants.FIGHTERS_TOWER_EXTERNAL_NAME;
                    break;
                /*todo implementar o resto*/
            }
        }

        if (Data.Type == Constants.RESOURCE)
        {
            switch (Data.ResourceType)
            {
                case Constants.SALMON:
                    Name = Constants.FISHERMAN_HOUSE_EXTERNAL_NAME;
                    break;
                case Constants.CATNIP:
                    Name = Constants.DISTILLERY_EXTERNAL_NAME;
                    break;
                case Constants.SAND:
                    Name = Constants.SAND_MINE_EXTERNAL_NAME;
                    break;
            }
        }

        MaxProgress = OS.IsDebugBuild() ? 3 : Data.MaxProgress;
        AboutToInteract += SimulationMode.Singleton.AboutToInteractWithBuilding;
        RemovedInteraction += SimulationMode.Singleton.InteractionWithBuildingRemoved;
        BuildMode.Singleton.ConstructionStarted += ConstructionStarted;
        BuildTickTimer = new Timer();
        BuildTickTimer.OneShot = true;
        BuildTickTimer.Timeout += ConstructionTimeElapsed;
        Destroyed += OnDestroyed;
        AddChild(BuildTickTimer);

        if (!IsPreSpawned)
        {
            return;
        }

        PlaceBuilding(this);
    }


    public void AddSelfOnList()
    {
        switch (Data.ResourceType)
        {
            case Constants.SALMON:
                ResourceType = Constants.SALMON;
                LevelManager.Singleton.SalmonBuildings.Add(this);
                break;
            case Constants.CATNIP:
                ResourceType = Constants.CATNIP;
                LevelManager.Singleton.CatnipBuildings.Add(this);
                break;
            case Constants.SAND:
                ResourceType = Constants.SAND;
                LevelManager.Singleton.SandBuildings.Add(this);
                break;
        }
    }

    public void RemoveSelfFromList()
    {
        switch (Data.ResourceType)
        {
            case Constants.SALMON:
                LevelManager.Singleton.SalmonBuildings.Remove(this);
                break;
            case Constants.CATNIP:
                LevelManager.Singleton.CatnipBuildings.Remove(this);
                break;
            case Constants.SAND:
                LevelManager.Singleton.SandBuildings.Remove(this);
                break;
        }
    }

    public async void ConstructionStarted(Building building)
    {
        if (building != this) return;

        Structure = building.IsRotated ? Data.Structure.RotatedProgressStructure : Data.Structure.ProgressStructure;
        Progress = 0;
        BuildTickTimer.Start(TickTime);
        Sounds.Stream = Data.PlaceBuildingSound;
        Sounds.Play();
        Placed = true;
        ModulateBuilding(building, BuildingInteractionStates.BUILD_PLACED);
        await ToSignal(Sounds, AudioStreamPlayer2D.SignalName.Finished);
    }
    private async void OnDestroyed()
    {
        Sounds.Stream = Data.DestroyBuildingSounds[GD.RandRange(0, Data.DestroyBuildingSounds.Count - 1)];
        Sounds.Play();
        await ToSignal(Sounds, AudioStreamPlayer2D.SignalName.Finished);
        QueueFree();
    }

    public void ConstructionTimeElapsed()
    {
        if (CurrentBuilders.Count == 0)
        {
            BuildTickTimer.Start(TickTime);
            return;
        }
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
        RemoveSelfFromList();
    }

    public static void ModulateBuilding(Building building, BuildingInteractionStates state)
    {
        switch (state)
        {
            case BuildingInteractionStates.HOVER:
                building.Modulate = building.Placed ? building.HoverColor : building.OkColor;
                break;
            case BuildingInteractionStates.UNHOVER:
                building.Modulate = building.Placed ? building.RegularColor : building.OkColor;
                break;
            case BuildingInteractionStates.BUILDING_ERROR:
                building.Modulate = building.ErrorColor;
                break;
            case BuildingInteractionStates.BUILDING_OK:
                building.Modulate = building.OkColor;
                break;
            case BuildingInteractionStates.BUILD_PLACED:
                building.Modulate = building.RegularColor;
                break;
        }
    }

    public static void PlaceBuilding(Building building)
    {
        building.Progress = building.MaxProgress;
        if (building.IsRotated)
        {
            building.Sprite.Offset = (building.Sprite.Offset + building.Data.Structure.RotatedPlacedOffset);
        }
        else
        {
            building.Sprite.Offset = (building.Sprite.Offset + building.Data.Structure.PlacedOffset);
        }

        building.AddSelfOnList();
        building.CurrentBuilders.Clear();
        building.IsBuilt = true;
    }

    public static void RotateBuildingPreview(Building building)
    {
        if (building.IsRotated)
        {
            building.Sprite.Texture = building.Data.Structure.PreviewTexture;
            building.BodyShape.Polygon = building.Data.Structure.Collision.Segments;
            building.InteractionShape.Polygon = building.Data.Structure.Interaction.Segments;
            building.Structure = building.Data.Structure.RotatedProgressStructure;
            building.IsRotated = false;
        }
        else
        {
            building.Sprite.Texture = building.Data.Structure.RotatedPreviewTexture;
            building.BodyShape.Polygon = building.Data.Structure.RotatedCollision.Segments;
            building.InteractionShape.Polygon = building.Data.Structure.RotatedInteraction.Segments;
            building.Structure = building.Data.Structure.ProgressStructure;
            building.IsRotated = true;
        }
    }

    public void ChangeSpriteOnBreakpoint()
    {
        if (Progress == 0)
        {
            SpriteHandler.ChangeSprite(
                Sprite,
                Structure.StaticTextures[(int)ProgressStructure.States.Empty],
                Structure.StaticOffsets[(int)ProgressStructure.States.Empty]
            );
        }
        else if (Progress >= 0.25 * MaxProgress && Progress <= 0.5 * MaxProgress)
        {
            SpriteHandler.ChangeSprite(
                Sprite,
                Structure.StaticTextures[(int)ProgressStructure.States.Low],
                Structure.StaticOffsets[(int)ProgressStructure.States.Low]
            );
        }
        else if (Progress >= 0.5 * MaxProgress && Progress <= 0.75 * MaxProgress)
        {
            SpriteHandler.ChangeSprite(
                Sprite,
                Structure.StaticTextures[(int)ProgressStructure.States.Medium],
                Structure.StaticOffsets[(int)ProgressStructure.States.Medium]
            );
        }
        else if (Progress == MaxProgress)
        {
            SpriteHandler.ChangeSprite(
                Sprite,
                Structure.StaticTextures[(int)ProgressStructure.States.Full],
                Structure.StaticOffsets[(int)ProgressStructure.States.Full]
            );
        }
    }
}