using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class TerrainBaking : Node
{
    public static TerrainBaking Singleton { get; private set; }
	public NavigationRegion2D CurrentRegion;
    public bool CurrentlyBaking { get; private set; }

    public override void _EnterTree()
    {
        Singleton = this;
    }

    public override void _Ready()
    {
        Initialize();
    }

    public void Initialize()
    {
        CurrentRegion = ModeManager.Singleton.Region;
        CurrentRegion.BakeFinished += FreeToRebake;
    }

    public async void Rebake()
    {
        CurrentRegion.BakeNavigationPolygon();
        await ToSignal(CurrentRegion, NavigationRegion2D.SignalName.BakeFinished);
    }

    public void FreeToRebake()
    {
        CurrentlyBaking = false;

        if (ModeManager.Singleton.BuildingsToBake.Count > 0)
        {
            RebakeAddBuilding(ModeManager.Singleton.BuildingsToBake[0]);
            Rebake();
            ModeManager.Singleton.BuildingsToBake.RemoveAt(0);
        }
    }

    public void RebakeAddBuilding(Building building, bool initialized = false)
    {
        if (ModeManager.Singleton.CurrentMode is not BuildMode && !initialized)
        {
            return;
        }

        SetRebakeBuilding(building);
    }

    public void RebakeRemoveBuilding(Building building)
    {
        if (!CurrentlyBaking)
        {
            StaticBody2D obstacle = CurrentRegion.GetNode<StaticBody2D>("Obstacle_Region_" + building.Data.Name + "_" + building.SelfIndex);
            obstacle.Reparent(building);
            Rebake();
            CurrentlyBaking = true;
        }
    }

    public void SetRebakeBuilding(Building building)
    {
        building.StaticBody.Name = "Obstacle_Region_" + building.Data.Name + "_" + building.SelfIndex;
        building.StaticBody.Reparent(CurrentRegion);
        CurrentlyBaking = true;
    }

    public void RebakeAddCollectionPoint(CollectPoint collectPoint) {
        collectPoint.Name = collectPoint.Name + "_" + collectPoint.SelfIndex;
        collectPoint.Reparent(CurrentRegion);
        CurrentlyBaking = true;
    }

}
