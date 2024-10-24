using Godot;

public partial class AddTower : Button
{
    public ModeManager ModeManager;
    public UI Ui;
    
    [Export] public BuildingData Building;

    public override void _Ready(){
        Ui = GetNode<UI>("/root/Game/UI");
        MouseEntered += Ui.Enter_ui_mode;
        MouseExited += Ui.Leave_ui_mode;
    }

    public void OnPressed() {
        if (ModeManager == null) {
            ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
        }
        ModeManager.CurrentMode.EmitSignal("ModeTransition", "BuildMode", Building.Type, Building.Name);
    }
}
