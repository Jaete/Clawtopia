using Godot;

public partial class BuildingMenu : Control
{
    public Ui Ui;
    public Building Building;
    public Button RemoveTowerButton;
    public ModeManager ModeManager;

    public override void _Ready() {
        Name = Constants.BUILDING_MENU;
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
        Ui = GetNode<Ui>("/root/Game/UI");
        RemoveTowerButton = GetNode<Button>("Button");
        RemoveTowerButton.Pressed += Remove_tower;
        MouseEntered += Ui.Enter_ui_mode;
        MouseExited += Ui.Leave_ui_mode;
    }

    public void Remove_tower() {
        Building.RebakeRemoveBuilding();
        Building.QueueFree();
        Ui.Reset_ui();
    }
}