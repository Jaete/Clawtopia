using System;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class UI : CanvasLayer
{
    public PackedScene BaseMenu = GD.Load<PackedScene>("res://TSCN/UI/BaseMenu.tscn");

    public Building Building;
    public PackedScene BuildingMenu = GD.Load<PackedScene>("res://TSCN/UI/BuildingMenu.tscn");
    public BuildingMenu BuildingMenuControl;
    public PackedScene CommunistMenu = GD.Load<PackedScene>("res://TSCN/UI/CommunistMenu.tscn");

    public HFlowContainer Container;


    public Control CurrentWindow;

    public bool IsResettingUi;
    public ModeManager ModeManager;

    private PauseMenu pauseMenu;
    public PackedScene PauseMenuScene = GD.Load<PackedScene>("res://TSCN/UI/PauseMenu.tscn");
    public PackedScene PurrlamentMenu = GD.Load<PackedScene>("res://TSCN/UI/PurrlamentMenu.tscn");

    public UIMode UiMode;

    public override void _Ready()
    {
        pauseMenu = PauseMenuScene.Instantiate<PauseMenu>();
        AddChild(pauseMenu);
        CallDeferred("Initialize");
        
        //Input.MouseMode = Input.MouseModeEnum.Confined;
    }

    public void Initialize()
    {
        Container = GetNode<HFlowContainer>("Container");
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
        UiMode = (UIMode)ModeManager.GameModes[GameMode.UI_MODE];
    }

    public void Instantiate_window(String window, Building building = null)
    {
        switch (window)
        {
            case Constants.BUILDING_MENU:
                if (building != null)
                {
                    BuildingMenuControl = BuildingMenu.Instantiate<BuildingMenu>();
                    BuildingMenuControl.Building = building;
                    BuildingMenuControl.Name = Constants.BUILDING_MENU;
                    CurrentWindow = BuildingMenuControl;
                }

                break;
            case Constants.PURRLAMENT_MENU:
                CurrentWindow = PurrlamentMenu.Instantiate<PurrlamentMenu>();
                CurrentWindow.Name = Constants.PURRLAMENT_MENU;
                break;
            case Constants.COMMUNIST_MENU:
                CurrentWindow = CommunistMenu.Instantiate<CommunistMenu>();
                CurrentWindow.Name = Constants.COMMUNIST_MENU;
                break;
        }

        var previousMenu = Container.GetChild<Control>(0);

        if (previousMenu.Name == CurrentWindow.Name)
        {
            return;
        }

        previousMenu.QueueFree();
        Container.AddChild(CurrentWindow);
    }

    public void Reset_ui()
    {
        IsResettingUi = true;
        var previousMenu = Container.GetChild<Control>(0);
        previousMenu.QueueFree();
        CurrentWindow = BaseMenu.Instantiate<Control>();
        CurrentWindow.Name = Constants.BASE_MENU;
        Container.AddChild(CurrentWindow);
        IsResettingUi = false;
    }

    public void EnterUiMode()
    {
        if (ModeManager.CurrentMode is not SimulationMode) { return; }
        var simulationMode = (SimulationMode)ModeManager.CurrentMode;
        if (simulationMode.Dragging) { return; }
        ModeManager.CurrentMode.EmitSignal(GameMode.SignalName.ModeTransition, GameMode.UI_MODE, "", "");
    }

    public void ExitUiMode()
    {
        if (ModeManager.CurrentMode is not UIMode) { return; }
        ModeManager.CurrentMode.EmitSignal(GameMode.SignalName.ModeTransition, GameMode.SIMULATION_MODE, "", "");
    }
}