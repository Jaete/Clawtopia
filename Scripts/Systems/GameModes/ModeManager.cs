using Godot;
using Godot.Collections;
using System;

public partial class ModeManager : Node2D
{
    public bool CurrentlyBaking = false;
    public Array<Building> BuildingsToBake = new();
    public NavigationRegion2D Region;

    public Dictionary GameModes = (Dictionary) new Dictionary<string, Variant>();
    public GameMode CurrentMode;
    public int BuildingCount;

    public string TowerType;
    
    public int FightersTowerCount;
    public int GreatCommuneCount;
    public int SalmonCottageCount;

    public String ResourceBuildType;
    public int ResourceBuildCount;

    public int HouseCount;

    public Director Director;
    public Node2D CurrentLevel;

    public override void _Ready() {
        Initialize();
    }

    public void Initialize() {
        //Director = GetNode<Director>("/root/Game/Director");
        CurrentLevel = GetNode<Node2D>("/root/Game/LevelManager/Level");
        SetGameModes();
        SetInitialBuildings();
        CurrentMode = (GameMode) GameModes["SimulationMode"];
    }

    public void ChangeMode(String mode, String building, String type) {
        CurrentMode.Exit();
        CurrentMode = (GameMode)GameModes[mode];
        if (CurrentMode is BuildMode buildMode) {
            buildMode.BuildingType = building;
            switch (buildMode.BuildingType) {
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

    public override void _Process(double delta) {
        CurrentMode.Update();
    }

    public void SetGameModes(){
        Array<Node> modes = GetChildren();
        foreach (var mode in modes) {
            if (mode is GameMode gameMode) {
                gameMode.ModeTransition += ChangeMode;
                GameModes[gameMode.Name] = gameMode;
            }
        }
    }

    public void SetInitialBuildings(){
        Array<Node> nodes = GetNode("/root/Game/LevelManager/Level").GetChildren();
        foreach (var node in nodes) {
            if (node is Building build){
                BuildingCount++;
                Building building = build;
                if (building.Data.Type == Constants.TOWER) {
                    if (building.Name.ToString().Contains(Constants.FIGHTERS)){
                        building.SelfIndex = FightersTowerCount;
                        FightersTowerCount++;
                    }
                }
                if (building.Data.Type == Constants.RESOURCE) { 
                    if (building.Name.ToString().Contains(Constants.COMMUNIST)){
                        building.SelfIndex = SalmonCottageCount;
                        SalmonCottageCount++;
                    }
                }
                if (building.Data.Type == Constants.COMMUNE) { 
                    building.SelfIndex = GreatCommuneCount;
                    GreatCommuneCount++;
                }
                BuildingsToBake.Add(building);
                building.RebakeAddBuilding();
            }
        }
        Region = GetNode<NavigationRegion2D>("/root/Game/LevelManager/Level/Navigation");
        Region.BakeNavigationPolygon();
        BuildingsToBake.Clear();
    }

}
