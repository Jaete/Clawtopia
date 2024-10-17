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

    public HFlowContainer container;

    public bool is_resetting_ui;

    public UIMode ui_mode;
    public ModeManager mode_manager;

    private PauseMenu pauseMenu;

    public override void _Ready()
    {
        pauseMenu = PauseMenuScene.Instantiate<PauseMenu>();
        AddChild(pauseMenu);
        CallDeferred("Initialize");
        Input.MouseMode = Input.MouseModeEnum.Confined;
    }

    public void Initialize(){
        container = GetNode<HFlowContainer>("Container");
        mode_manager = GetNode<ModeManager>("/root/Game/ModeManager");
        ui_mode = (UIMode) mode_manager.GameModes["UIMode"];
    }
    
    public void Instantiate_window(String window, Building building = null) {
        switch (window) {
            case "BuildingMenu":
                if (building != null){
                    building_menu_control = building_menu.Instantiate<BuildingMenu>();
                    building_menu_control.Building = building;
                    building_menu_control.Name = "BuildingMenu";
                    current_window = building_menu_control;
                }
                break;
            case "CommunistMenu":
                current_window = communist_menu.Instantiate<CommunistMenu>();
                current_window.Name = "CommunistMenu";
                break;
        }
        var previous_menu = GetNodeOrNull("Container").GetChild<Control>(0);
        if (previous_menu.Name == current_window.Name){
            return;
        }
        previous_menu.QueueFree();
        container.AddChild(current_window);
    }
    
    public void Reset_ui(){
        is_resetting_ui = true;
        var previous_menu = GetNodeOrNull("Container").GetChild<Control>(0);
        previous_menu.QueueFree();
        current_window = base_menu.Instantiate<Control>();
        current_window.Name = "BaseMenu";
        container.AddChild(current_window);
        is_resetting_ui = false;
    }

    public void Enter_ui_mode(){
        if (mode_manager.CurrentMode is SimulationMode){
            mode_manager.ChangeMode(ui_mode.Name, "", "");
        }
    }

    public void Leave_ui_mode(){
        if (mode_manager.CurrentMode is UIMode) {
            var simulation_mode = (SimulationMode) mode_manager.GameModes["SimulationMode"];
            mode_manager.ChangeMode(simulation_mode.Name, "", "");
        }
    }
}
