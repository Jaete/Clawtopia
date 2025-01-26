using System;
using Godot;
using Godot.Collections;

namespace ClawtopiaCs.Scripts.Systems.GameModes;

public partial class ModeManager : Node2D
{
    public int BuildingCount;
    public Array<Building> BuildingsToBake = new();

    public Controller Controller;
    public Node2D CurrentLevel;
    public bool CurrentlyBaking = false;
    public GameMode CurrentMode;

    public Director Director;

    public int FightersTowerCount;

    public Dictionary GameModes = (Dictionary)new Dictionary<string, Variant>();
    public int GreatCommuneCount;

    public int HouseCount;
    public NavigationRegion2D Region;
    public int ResourceBuildCount;

    public String ResourceBuildType;
    public int SalmonCottageCount;

    public string TowerType;

    public override void _Ready()
    {
        Initialize();
    }

    public void Initialize()
    {
        //Director = GetNode<Director>("/root/Game/Director");
        CurrentLevel = GetNode<Node2D>("/root/Game/LevelManager/Level");
        Controller = GetNode<Controller>("/root/Game/Controller");
        SetGameModes();
        SetInitialBuildings();
        CurrentMode = (GameMode)GameModes["SimulationMode"];
        Controller.MousePressed += MousePressed;
        Controller.MouseReleased += MouseReleased;
        Controller.MouseRightPressed += MouseRightPressed;
    }

    public void ChangeMode(string mode, string building, string type)
    {
        CurrentMode.Exit();
        CurrentMode = (GameMode)GameModes[mode];

        if (CurrentMode is BuildMode buildMode)
        {
            buildMode.BuildingType = building;

            switch (buildMode.BuildingType)
            {
                case Constants.TOWER:
                    TowerType = type;
                    break;
                case Constants.RESOURCE:
                    ResourceBuildType = type;
                    break;
            }
        }

        CurrentMode.Enter();
    }

    public override void _Process(double delta)
    {
        CurrentMode.Update();
    }

    public void MousePressed(Vector2 coords)
    {
        CurrentMode.MousePressed(coords);
    }

    public void MouseReleased(Vector2 coords)
    {
        CurrentMode.MouseReleased(coords);
    }

    public void MouseRightPressed(Vector2 coords)
    {
        CurrentMode.MouseRightPressed(coords);
    }


    public void SetGameModes()
    {
        Array<Node> modes = GetChildren();

        foreach (var mode in modes)
        {
            if (mode is GameMode gameMode)
            {
                gameMode.ModeTransition += ChangeMode;
                GameModes[gameMode.Name] = gameMode;
            }
        }
    }

    public void SetInitialBuildings()
    {
        Array<Node> nodes = CurrentLevel.GetChildren();

        foreach (var node in nodes)
        {
            if (node is Building build)
            {
                BuildingCount++;
                Building building = build;

                if (building.Data.Type == Constants.TOWER)
                {
                    if (building.Name.ToString().Contains(Constants.FIGHTERS))
                    {
                        building.SelfIndex = FightersTowerCount;
                        FightersTowerCount++;
                    }
                }

                if (building.Data.Type == Constants.RESOURCE)
                {
                    if (building.Name.ToString().Contains(Constants.COMMUNIST))
                    {
                        building.SelfIndex = SalmonCottageCount;
                        SalmonCottageCount++;
                    }
                }

                if (building.Data.Type == Constants.COMMUNE)
                {
                    building.SelfIndex = GreatCommuneCount;
                    GreatCommuneCount++;
                }

                BuildingsToBake.Add(building);
                building.RebakeAddBuilding(true);
            }
        }

        Region = CurrentLevel.GetNode<NavigationRegion2D>("Navigation");
        Region.BakeNavigationPolygon();
        BuildingsToBake.Clear();
    }
}