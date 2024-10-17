using Godot;
using System;

public partial class UI : CanvasLayer
{
    
    public PackedScene PauseMenuScene = GD.Load<PackedScene>("res://TSCN/UI/PauseMenu.tscn");
    public PackedScene building_menu = GD.Load<PackedScene>("res://TSCN/UI/BuildingMenu.tscn");
    public PackedScene communist_menu = GD.Load<PackedScene>("res://TSCN/UI/CommunistMenu.tscn");
    public PackedScene base_menu = GD.Load<PackedScene>("res://TSCN/UI/BaseMenu.tscn");
    

    public Control current_window;
    public BuildingMenu building_menu_control;

    public Building Building;

    public HFlowContainer Container;

    public bool IsResettingUi;

    public UIMode UiMode;
    public ModeManager ModeManager;

    public override void _Ready(){
        CallDeferred("Initialize");
        Input.MouseMode = Input.MouseModeEnum.Confined;
    }

    public void Initialize(){
        Container = GetNode<HFlowContainer>("Container");
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
        UiMode = (UIMode) ModeManager.GameModes["UIMode"];
    }
    
    public void Instantiate_window(String window, Building building = null) {
        switch (window) {
            case Constants.BUILDING_MENU:
                if (building != null){
                    building_menu_control = building_menu.Instantiate<BuildingMenu>();
                    building_menu_control.Building = building;
                    building_menu_control.Name = "BuildingMenu";
                    current_window = building_menu_control;
                }
                break;
            case Constants.COMMUNIST_MENU:
                CurrentWindow = CommunistMenu.Instantiate<CommunistMenu>();
                CurrentWindow.Name = Constants.COMMUNIST_MENU;
                break;
        }
        var previousMenu = GetNodeOrNull("Container").GetChild<Control>(0);
        if (previousMenu.Name == CurrentWindow.Name){
            return;
        }
        previousMenu.QueueFree();
        Container.AddChild(CurrentWindow);
    }
    
    public void Reset_ui(){
        IsResettingUi = true;
        var previousMenu = GetNodeOrNull("Container").GetChild<Control>(0);
        previousMenu.QueueFree();
        CurrentWindow = BaseMenu.Instantiate<Control>();
        CurrentWindow.Name = Constants.BASE_MENU;
        Container.AddChild(CurrentWindow);
        IsResettingUi = false;
    }

    public void Enter_ui_mode(){
        if (ModeManager.CurrentMode is SimulationMode){
            ModeManager.ChangeMode(UiMode.Name, "", "");
        }
    }

    public void Leave_ui_mode(){
        if (ModeManager.CurrentMode is UIMode) {
            var simulationMode = (SimulationMode) ModeManager.GameModes["SimulationMode"];
            ModeManager.ChangeMode(simulationMode.Name, "", "");
        }
    }
}
