using Godot;
using System;

public partial class UI : CanvasLayer
{
    
    public PackedScene PauseMenuScene = GD.Load<PackedScene>("res://TSCN/UI/PauseMenu.tscn");
    public PackedScene BuildingMenu = GD.Load<PackedScene>("res://TSCN/UI/BuildingMenu.tscn");
    public PackedScene CommunistMenu = GD.Load<PackedScene>("res://TSCN/UI/CommunistMenu.tscn");
    public PackedScene BaseMenu = GD.Load<PackedScene>("res://TSCN/UI/BaseMenu.tscn");
    

    public Control CurrentWindow;
    public BuildingMenu BuildingMenuControl;

    public Building Building;

    public HFlowContainer Container;

    public bool IsResettingUi;

    public UIMode UiMode;
    public ModeManager ModeManager;

    private PauseMenu pauseMenu;

    public override void _Ready(){
        pauseMenu = PauseMenuScene.Instantiate<PauseMenu>();
        AddChild(pauseMenu);
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
                    BuildingMenuControl = BuildingMenu.Instantiate<BuildingMenu>();
                    BuildingMenuControl.Building = building;
                    BuildingMenuControl.Name = "BuildingMenu";
                    CurrentWindow = BuildingMenuControl;
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
