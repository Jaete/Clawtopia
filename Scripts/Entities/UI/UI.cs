using System;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class UI : CanvasLayer
{
    public static UI Singleton;

    public PackedScene BaseMenu = GD.Load<PackedScene>("res://TSCN/UI/Menu/BaseMenu.tscn");
    public PackedScene HouseMenu = GD.Load<PackedScene>("res://TSCN/UI/Menu/Buildings/HouseMenu.tscn");
    public PackedScene CatnipFarmMenu = GD.Load<PackedScene>("res://TSCN/UI/Menu/Buildings/CatnipFarmMenu.tscn");
    public PackedScene CommunistMenu = GD.Load<PackedScene>("res://TSCN/UI/Menu/Characters/CommunistMenu.tscn");
    public PackedScene UnitMenu = GD.Load<PackedScene>("res://TSCN/UI/Menu/Characters/UnitMenu.tscn");
    public PackedScene PauseMenuScene = GD.Load<PackedScene>("res://TSCN/UI/Menu/PauseMenu.tscn");
    public PackedScene PurrlamentMenu = GD.Load<PackedScene>("res://TSCN/UI/Menu/Buildings/PurrlamentMenu.tscn");
    public PackedScene SalmonCottageMenu = GD.Load<PackedScene>("res://TSCN/UI/Menu/Buildings/SalmonCottageMenu.tscn");
    public PackedScene SandMineMenu = GD.Load<PackedScene>("res://TSCN/UI/Menu/Buildings/SandMineMenu.tscn");



    public Building Building;
    public BuildingMenu BuildingMenuControl;
    public HFlowContainer Container;
    public Control CurrentWindow;
    public UIMode UiMode;
    private PauseMenu pauseMenu;
    
    public bool IsResettingUi;

    public override void _Ready()
    {
        Singleton = this;
        pauseMenu = PauseMenuScene.Instantiate<PauseMenu>();
        AddChild(pauseMenu);
        CallDeferred("Initialize");
        
        //Input.MouseMode = Input.MouseModeEnum.Confined;
    }

    public void Initialize()
    {
        Container = GetNode<HFlowContainer>("Container");
        UiMode = (UIMode)ModeManager.Singleton.GameModes[GameMode.UI_MODE];
    }

    public void Instantiate_window(String window, Building building = null)
    {
        switch (window)
        { 
            case Constants.HOUSE_MENU:
                if (building != null)
                {
                    BuildingMenuControl = HouseMenu.Instantiate<HouseMenu>();
                    BuildingMenuControl.Building = building;
                    BuildingMenuControl.Name = Constants.HOUSE_MENU;
                    CurrentWindow = BuildingMenuControl;
                }
                break;
            case Constants.SALMON_MENU:
                if (building != null)
                {
                    BuildingMenuControl = SalmonCottageMenu.Instantiate<SalmonCottageMenu>();
                    BuildingMenuControl.Building = building;
                    BuildingMenuControl.Name = Constants.SALMON_MENU;
                    CurrentWindow = BuildingMenuControl;
                }
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
        if (ModeManager.Singleton.CurrentMode is not SimulationMode) { return; }
        if (SimulationMode.Singleton.Dragging) { return; }
        SimulationMode.Singleton.EmitSignal(GameMode.SignalName.ModeTransition, GameMode.UI_MODE, "", "");
    }

    public void ExitUiMode()
    {
        if (ModeManager.Singleton.CurrentMode is not UIMode) { return; }
        EmitSignal(GameMode.SignalName.ModeTransition, GameMode.SIMULATION_MODE, "", "");
    }
}