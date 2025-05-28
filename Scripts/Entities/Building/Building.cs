using System;
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

    [Signal]
    public delegate void DestroyedEventHandler ();

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

    [Export] public bool IsPreSpawned = false;

    public Color ErrorColor = new Color("ba000079");
    public Color HoverColor = new Color(1.3f, 1.3f, 1.3f);
    public Color OkColor = new Color("2eff3f81");
    public Color RegularColor = new Color(1, 1, 1);

    public bool IsBuildingInFront;
    public bool IsBuilt;
    public bool Placed;
    public bool IsRotated;

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
        CallDeferred(MethodName.Initialize);
    }

    public void Initialize()
    { 
        StaticBody = GetNode<StaticBody2D>("NavigationBody");
        Data.Initialize();
        Sprite.Texture = Data.Structure.PreviewTexture;
        BodyShape.Polygon = Data.Structure.Collision.Segments;
        InteractionShape.Polygon = Data.Structure.Interaction.Segments;
        GridShape.Polygon = Data.Structure.GridArea.Segments;

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
        Progress = 0;
        BuildTickTimer.Start(TickTime);
        Sounds.Stream = Data.PlaceBuildingSound;
        Sounds.Play();
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

    public static void PlaceBuilding(Building building)
    {
        if (building.IsRotated)
        {
            building.Sprite.Texture = building.Data.Structure.RotatedPlacedTexture;
            building.Sprite.Offset = (building.Sprite.Offset + building.Data.Structure.RotatedPlacedOffset);
        }
        else
        {
            building.Sprite.Texture = building.Data.Structure.PlacedTexture;
            building.Sprite.Offset = (building.Sprite.Offset + building.Data.Structure.PlacedOffset);
        }
       
        ModulateBuilding(building, BuildingInteractionStates.BUILD_FINISHED);

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
            building.GridShape.Polygon = building.Data.Structure.GridArea.Segments;
            building.IsRotated = false;
        }
        else
        {
            building.Sprite.Texture = building.Data.Structure.RotatedPreviewTexture;
            building.BodyShape.Polygon = building.Data.Structure.RotatedCollision.Segments;
            building.InteractionShape.Polygon = building.Data.Structure.RotatedInteraction.Segments;
            building.GridShape.Polygon = building.Data.Structure.RotatedGridArea.Segments;
            building.IsRotated = true;
        }
    }
}