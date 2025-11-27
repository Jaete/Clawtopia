using System;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class UI : CanvasLayer
{
    public static UI Singleton;

    public PackedScene BaseMenu = GD.Load<PackedScene>("res://TSCN/UI/Menu/BaseMenu.tscn");
    public PackedScene PauseMenuScene = GD.Load<PackedScene>("res://TSCN/UI/Menu/PauseMenu.tscn");

    public HFlowContainer Container;
    public Control CurrentWindow;
    public UIMode UiMode;
    private PauseMenu pauseMenu;
    
    public bool IsResettingUi;

    public override void _EnterTree()
    {
        Singleton = this;
    }

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
        UiMode = (UIMode)ModeManager.Singleton.GameModes[GameMode.UI_MODE];
    }

    public static void OpenMenu(Node2D entity)
    {
        if (entity is Building building)
        {
            var buildingMenu = GD.Load<PackedScene>(building.Data.UIMenu).Instantiate<BuildingMenu>();
            buildingMenu.BuildingLevelID = building.GetInstanceId();
            buildingMenu.Name = building.Name + "_Menu";
            Singleton.CurrentWindow = buildingMenu;
        }

        if (entity is Ally unit)
        {
            var allyMenu = unit.UIMenu.Instantiate<AllyMenu>();
            allyMenu.Name = unit.Name + "_Menu";
            Singleton.CurrentWindow = allyMenu;
        }

        if (entity is Area2D area && area.GetParent() is CollectPoint collectPoint)
        {
            PackedScene menuScene = null;

            switch (collectPoint.Resource.Name)
            {
                case Constants.CATNIP:
                    menuScene = GD.Load<PackedScene>("res://TSCN/UI/Menu/Resources/CatnipResourceMenu.tscn");
                    break;
                case Constants.SALMON:
                    menuScene = GD.Load<PackedScene>("res://TSCN/UI/Menu/Resources/SalmonResourceMenu.tscn");
                    break;
                case Constants.SAND:
                    menuScene = GD.Load<PackedScene>("res://TSCN/UI/Menu/Resources/SandResourceMenu.tscn");
                    break;
            }

            if (menuScene == null)
                return;
            
            var resourceMenu = menuScene.Instantiate<ResourceMenu>();
            resourceMenu.SetCollectPoint(collectPoint);
            resourceMenu.Name = collectPoint.Name + "_Resource";
            Singleton.CurrentWindow = resourceMenu;
        }

        var previousMenu = Singleton.Container.GetChild<Control>(0);

        if (previousMenu.Name == Singleton.CurrentWindow.Name)
        {
            return;
        }

        previousMenu.QueueFree();
        Singleton.Container.AddChild(Singleton.CurrentWindow);
    }

    public static void ResetUI()
    {
        Singleton.IsResettingUi = true;
        var previousMenu = Singleton.Container.GetChild<Control>(0);
        previousMenu.QueueFree();
        Singleton.CurrentWindow = Singleton.BaseMenu.Instantiate<Control>();
        Singleton.CurrentWindow.Name = Constants.BASE_MENU;
        Singleton.Container.AddChild(Singleton.CurrentWindow);
        Singleton.IsResettingUi = false;
    }

    public static void EnterUIMode()
    {
        if (ModeManager.Singleton.CurrentMode is not SimulationMode) { return; }
        if (SimulationMode.Singleton.Dragging) { return; }
        SimulationMode.Singleton.EmitSignal(GameMode.SignalName.ModeTransition, GameMode.UI_MODE, "", "");
    }

    public static void ExitUIMode()
    {
        if (ModeManager.Singleton.CurrentMode is not UIMode) { return; }
        Singleton.EmitSignal(GameMode.SignalName.ModeTransition, GameMode.SIMULATION_MODE, "", "");
    }
}