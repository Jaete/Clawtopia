using Godot;
using System;

public partial class BuildingMenu : BaseMenu
{
    [ExportGroup("Settings")]
    [Export] public Button RemoveBuildingButton;
    [Export] public BuildingHpBar hpBar;

    public ulong BuildingLevelID;
    private Building _building;

    public override void _Ready()
    {
        base._Ready();
        _building = (Building)InstanceFromId(BuildingLevelID);
        
        GD.Print("Opening ", this.Name);
        GD.Print("Building: ", _building?.Type ?? "Not defined");
        GD.Print("HPBar node exists: ", hpBar != null);
        
        if (_building != null && hpBar != null)
            hpBar.SetBuilding(_building);
        
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