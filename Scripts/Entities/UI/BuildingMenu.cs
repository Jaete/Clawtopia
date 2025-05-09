using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class BuildingMenu : Control
{
    [Export] public Button RemoveBuildingButton;
    
    public Building Building;
    public ModeManager ModeManager;
    public UI Ui;

    public override void _Ready()
    {
        Name = Constants.BUILDING_MENU;
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
        Ui = GetNode<UI>("/root/Game/UI");
        RemoveBuildingButton.Pressed += RemoveBuilding;
        MouseEntered += Ui.EnterUiMode;
        MouseExited += Ui.ExitUiMode;
    }

    public void RemoveBuilding()
    {
        TerrainBaking.Singleton.RebakeRemoveBuilding(Building);
        Building.EmitSignal(Building.SignalName.Destroyed);
        Ui.Reset_ui();
    }
}