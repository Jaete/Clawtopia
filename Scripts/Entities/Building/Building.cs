using Godot;
using Godot.Collections;

public partial class Building : Area2D
{

    [Signal]
    public delegate void AboutToInteractEventHandler(Building self);
    [Signal]
    public delegate void RemovedInteractionEventHandler();
    
    public Color OkColor = new Color("2eff3f81");
    public Color ErrorColor = new Color("ba000079");
    public Color RegularColor = new Color(1, 1, 1);
    public Color HoverColor = new Color(1.3f, 1.3f, 1.3f);
    
    public NavigationRegion2D Region;
    public SimulationMode SimulationMode;
    public BuildMode BuildingMode;
    public ModeManager ModeManager;
    public LevelManager LevelManager;
    
    public int SelfIndex;
    public bool Placed;
    public bool IsBuilt;
    
    // TIMER PARA TICK DE TEMPO DE CONSTRUCAO
    public SceneTreeTimer BuildTickTimer;
    // TEMPO EM SEGUNDOS POR TICK
    public float TickTime = 1.0f;
    public int Progress;
    public int MaxProgress = 50;
    public Array<Ally> CurrentBuilders = new();

    [Export] public bool IsPreSpawned = false;
    [Export] public BuildingData Data;
    [Export] public StaticBody2D StaticBody;
    [Export] public CollisionPolygon2D BodyShape;
    [Export] public CollisionPolygon2D InteractionShape;
    [Export] public CollisionPolygon2D GridShape;
    [Export] public Sprite2D Sprite;

    public string ResourceType;


    public override void _Ready() {
        Initialize();
    }
    
    public void Initialize() {
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
        BuildingMode = GetNode<BuildMode>("/root/Game/ModeManager/BuildMode");
        Region = GetNode<NavigationRegion2D>("../Navigation");
        StaticBody = GetNode<StaticBody2D>("NavigationBody");
        SimulationMode = GetNode<SimulationMode>("/root/Game/ModeManager/SimulationMode");
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
        if (Data.NeedsRegion){
            Sprite.RegionEnabled = true;
            Sprite.RegionRect = Data.RegionRect;
        }
        Name = Data.Name + "_" + Data.Type + "_" + SelfIndex;
        if (Data.Type.Equals(Constants.COMMUNE)){
            Name = Constants.COMMUNE_EXTERNAL_NAME;
        }
        if(Data.Type == Constants.HOUSE){
            Name = Constants.HOUSE_EXTERNAL_NAME;
        }
        if(Data.Type == Constants.TOWER){
            switch(Data.TowerType){
                case Constants.FIGHTERS:
                    Name = Constants.FIGHTERS_TOWER_EXTERNAL_NAME;
                    break;
                /*todo implementar o resto*/
            }
        }
        if(Data.Type == Constants.RESOURCE){
            switch(Data.ResourceType){
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
        MaxProgress = Data.MaxProgress;
        Region.BakeFinished += FreeToRebake;
        AboutToInteract += SimulationMode.AboutToInteractWithBuilding;
        RemovedInteraction += SimulationMode.InteractionWithBuildingRemoved;
        BuildingMode.ConstructionStarted += ConstructionStarted;
        CallDeferred("Add_self_on_list");
    }

    public void AddSelfOnList(){
        switch (Data.ResourceType){
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

    public void RemoveSelfFromList(){
        switch (Data.ResourceType){
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

    public void SetRebake() {
        StaticBody.Name = "Obstacle_Region_" + Data.Type + "_" + SelfIndex;
        StaticBody.Reparent(Region);
        ModeManager.CurrentlyBaking = true;
    }

    public void RebakeAddBuilding() {
        if (IsPreSpawned) {
            SetRebake();
            IsBuilt = true;
            return;
        }
        if (ModeManager.CurrentMode is BuildMode) {
            SetRebake();
            return;
        }
    }

    public void RebakeRemoveBuilding() {
        if (!ModeManager.CurrentlyBaking) {
            StaticBody2D obstacle = Region.GetNode<StaticBody2D>("Obstacle_Region_" + Data.Type + "_" + SelfIndex);
            obstacle.Reparent(this);
            Rebake();
            ModeManager.CurrentlyBaking = true;
        }
    }

    public void Rebake(){
        Region.BakeNavigationPolygon();
    }

    public void FreeToRebake() {
        ModeManager.CurrentlyBaking = false;
        Placed = true;
        if(ModeManager.BuildingsToBake.Count > 0) {
            ModeManager.BuildingsToBake[0].RebakeAddBuilding();
            ModeManager.BuildingsToBake[0].Rebake();
            ModeManager.BuildingsToBake.RemoveAt(0);
        }
    }

    public void ConstructionStarted(Building building){
        // if (IsBuilt){ return; }
        Progress = 0;
        BuildTickTimer = GetTree().CreateTimer(TickTime);
        BuildTickTimer.Timeout += ConstructionTimeElapsed;
    }
    public void ConstructionTimeElapsed(){
        // if (IsBuilt){ return; }
        var nextProgress = Progress + CurrentBuilders.Count;
        if (nextProgress < MaxProgress){
            Progress = nextProgress;
            BuildTickTimer = GetTree().CreateTimer(TickTime);
            BuildTickTimer.Timeout += ConstructionTimeElapsed;
            return;
        }
        BuildingMode.EmitSignal("BuildCompleted", this);
        IsBuilt = true;
        BuildTickTimer.Timeout -= ConstructionTimeElapsed;
    }

    public override void _MouseEnter(){
        if (ModeManager.CurrentMode is SimulationMode){
            Modulate = IsBuilt ? HoverColor : OkColor;
            if (SimulationMode.SelectedAllies.Count > 0){
                EmitSignal("AboutToInteract", this);
            }
        }
    }
    public override void _MouseExit(){
        if (ModeManager.CurrentMode is SimulationMode){
            Modulate = IsBuilt ? RegularColor : OkColor;
            if (SimulationMode.SelectedAllies.Count > 0){
                EmitSignal("RemovedInteraction");
            }
        }
    }

    public override void _ExitTree(){
        RemoveSelfFromList();
    }
}
