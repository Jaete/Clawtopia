using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class BuildingMenu : Control
{
    public Building Building;
    public ModeManager ModeManager;
    public Button RemoveBuildingButton;
    public UI Ui;

    public override void _Ready()
    {
        Name = Constants.BUILDING_MENU;
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
        Ui = GetNode<UI>("/root/Game/UI");
        RemoveBuildingButton = GetNode<Button>("RemoveBuilding/Button");
        RemoveBuildingButton.Pressed += RemoveBuilding;
        MouseEntered += Ui.EnterUiMode;
        MouseExited += Ui.ExitUiMode;
    }

    public void RemoveBuilding()
    {
        Building.RebakeRemoveBuilding();
        Building.EmitSignal(Building.SignalName.Destroyed);
        Ui.Reset_ui();
    }
}