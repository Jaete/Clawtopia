using Godot;

public partial class BuildingMenu : BaseMenu
{
    [ExportGroup("Settings")]
    [Export] public Button RemoveBuildingButton;

    public ulong BuildingLevelID;

    public override void _Ready()
    {
        base._Ready();
        if (RemoveBuildingButton is null) return;
        RemoveBuildingButton.Pressed += RemoveBuilding;
    }

    public void RemoveBuilding()
    {
        var building = (Building)InstanceFromId(BuildingLevelID);
        TerrainBaking.Singleton.RebakeRemoveBuilding(building);
        building.EmitSignal(Building.SignalName.Destroyed);
        UI.ResetUI();
    }
}