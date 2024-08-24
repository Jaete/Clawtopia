using Godot;
using Godot.Collections;

public partial class Building : Area2D
{

    [Signal]
    public delegate void AboutToInteractEventHandler(Building self);
    [Signal]
    public delegate void RemovedInteractionEventHandler();
    
    public Color OK_COLOR = new Color("2eff3f81");
    public Color ERROR_COLOR = new Color("ba000079");
    public Color REGULAR_COLOR = new Color(1, 1, 1);
    public Color HOVER_COLOR = new Color(1.3f, 1.3f, 1.3f);
    
    public NavigationRegion2D region;
    public SimulationMode simulation_mode;
    public BuildMode building_mode;
    public ModeManager mode_manager;
    public LevelManager level_manager;
    
    public int self_index;
    public bool placed;
    public bool is_built;
    
    // TIMER PARA TICK DE TEMPO DE CONSTRUCAO
    public SceneTreeTimer build_tick_timer;
    // TEMPO EM SEGUNDOS POR TICK
    public float TICK_TIME = 1.0f;
    public int progress;
    public int max_progress = 50;
    public Array<Ally> current_builders = new();

    [Export] public bool is_pre_spawned = false;
    [Export] public BuildingData data;
    [Export] public StaticBody2D static_body;
    [Export] public CollisionPolygon2D body_shape;
    [Export] public CollisionPolygon2D interaction_shape;
    [Export] public CollisionPolygon2D grid_shape;
    [Export] public Sprite2D sprite;

    public string resource_type;


    public override void _Ready() {
        Initialize();
    }
    
    public void Initialize() {
        mode_manager = GetNode<ModeManager>("/root/Game/ModeManager");
        building_mode = GetNode<BuildMode>("/root/Game/ModeManager/BuildMode");
        region = GetNode<NavigationRegion2D>("../Navigation");
        static_body = GetNode<StaticBody2D>("NavigationBody");
        simulation_mode = GetNode<SimulationMode>("/root/Game/ModeManager/SimulationMode");
        level_manager = GetNode<LevelManager>("/root/Game/LevelManager");
        data.initialize();
        body_shape.Polygon = data.OBSTACLE_SHAPE.Segments;
        interaction_shape.Polygon = data.INTERACTION_SHAPE.Segments;
        interaction_shape.Position = data.INTERACTION_OFFSET;
        grid_shape.Polygon = data.GRID_SHAPE.Segments;
        sprite.Texture = data.SPRITE_TEXTURE;
        sprite.Offset = data.OFFSET;
        sprite.Scale = data.SCALE;
        sprite.RegionEnabled = false;
        if (data.NEEDS_REGION){
            sprite.RegionEnabled = true;
            sprite.RegionRect = data.REGION_RECT;
        }
        Name = data.NAME + "_" + data.TYPE + "_" + self_index;
        if (data.TYPE.Equals("GreatCommune")){
            Name = data.TYPE;
        }
        max_progress = data.max_progress;
        region.BakeFinished += When_free_to_rebake;
        AboutToInteract += simulation_mode.When_about_to_interact_with_building;
        RemovedInteraction += simulation_mode.When_interaction_with_building_removed;
        building_mode.ConstructionStarted += When_construction_started;
        CallDeferred("Add_self_on_list");
    }

    public void Add_self_on_list(){
        switch (data.RESOURCE_TYPE){
            case Constants.SALMON:
                resource_type = Constants.SALMON;
                level_manager.salmon_buildings.Add(this);
                break;
            case Constants.CATNIP:
                resource_type = Constants.CATNIP;
                level_manager.catnip_buildings.Add(this);
                break;
            case Constants.SAND:
                resource_type = Constants.SAND;
                level_manager.sand_buildings.Add(this);
                break;
            default:
                resource_type = null;
                break;
        }   
    }

    public void Remove_self_from_list(){
        switch (data.RESOURCE_TYPE){
            case Constants.SALMON:
                level_manager.salmon_buildings.Remove(this);
                break;
            case Constants.CATNIP:
                level_manager.catnip_buildings.Remove(this);
                break;
            case Constants.SAND:
                level_manager.sand_buildings.Remove(this);
                break;
        }
    }

    public void Set_rebake() {
        static_body.Name = "Obstacle_Region_" + data.TYPE + "_" + self_index;
        static_body.Reparent(region);
        mode_manager.currently_baking = true;
    }

    public void Rebake_add_building() {
        if (is_pre_spawned) {
            Set_rebake();
            is_built = true;
            return;
        }
        if (mode_manager.current_mode is BuildMode) {
            Set_rebake();
            return;
        }
    }

    public void Remove_building_and_rebake() {
        if (!mode_manager.currently_baking) {
            StaticBody2D obstacle = region.GetNode<StaticBody2D>("Obstacle_Region_" + data.TYPE + "_" + self_index);
            obstacle.Reparent(this);
            Rebake();
            mode_manager.currently_baking = true;
        }
    }

    public void Rebake(){
        region.BakeNavigationPolygon();
    }

    public void When_free_to_rebake() {
        mode_manager.currently_baking = false;
        placed = true;
        if(mode_manager.buildings_to_bake.Count > 0) {
            mode_manager.buildings_to_bake[0].Rebake_add_building();
            mode_manager.buildings_to_bake[0].Rebake();
            mode_manager.buildings_to_bake.RemoveAt(0);
        }
    }

    public void When_construction_started(Building building){
        if (is_built){ return; }
        progress = 0;
        build_tick_timer = GetTree().CreateTimer(TICK_TIME);
        build_tick_timer.Timeout += When_construction_time_elapsed;
    }
    public void When_construction_time_elapsed(){
        if (is_built){ return;}
        var next_progress = progress + current_builders.Count;
        if (next_progress < max_progress){
            progress = next_progress;
            build_tick_timer = GetTree().CreateTimer(TICK_TIME);
            build_tick_timer.Timeout += When_construction_time_elapsed;
            return;
        }
        building_mode.EmitSignal("BuildCompleted", this);
        is_built = true;
    }

    public override void _MouseEnter(){
        if (mode_manager.current_mode is SimulationMode){
            Modulate = is_built ? HOVER_COLOR : OK_COLOR;
            if (simulation_mode.selected_allies.Count > 0){
                EmitSignal("AboutToInteract", this);
            }
        }
    }
    public override void _MouseExit(){
        if (mode_manager.current_mode is SimulationMode){
            Modulate = is_built ? REGULAR_COLOR : OK_COLOR;
            if (simulation_mode.selected_allies.Count > 0){
                EmitSignal("RemovedInteraction");
            }
        }
    }

    public override void _ExitTree(){
        Remove_self_from_list();
    }
}
