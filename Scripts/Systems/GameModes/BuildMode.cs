using Godot;
using System;
using Godot.Collections;

public partial class BuildMode : GameMode {

    public int TileSizeX = 64;
    public int TileSizeY = 32;

    public Vector2 MousePosition;

    public string BuildingType;

    public Building CurrentBuilding;
    public bool IsOverlappingBuildings = false;

    public Array<Ally> CurrentConstructors;
    public override void Enter() {
        String buildingPath = Constants.BUILDING_PATH;
        if(BuildingType == Constants.TOWER) {
            ModeManager.FightersTowerCount++;
            ModeManager.BuildingCount++;
            Instantiate_building(buildingPath);
            CurrentBuilding.SelfIndex = ModeManager.FightersTowerCount;
            CurrentBuilding.Name = ModeManager.TowerType + "_T1_" + CurrentBuilding.SelfIndex;
            CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/Towers/Fighters/Fighters.tres");
        }
        if(BuildingType == Constants.COMMUNE){
           ModeManager.GreatCommuneCount++;
           ModeManager.BuildingCount++;
           Instantiate_building(buildingPath);
           CurrentBuilding.SelfIndex = ModeManager.GreatCommuneCount;
           CurrentBuilding.Name = Constants.COMMUNE;   
           CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/GreatCommune/GreatCommune.tres");
        }    
        if(BuildingType == Constants.RESOURCE){
            ModeManager.SalmonCottageCount++;
            ModeManager.BuildingCount++;
            Instantiate_building(buildingPath);
            CurrentBuilding.SelfIndex = ModeManager.SalmonCottageCount;
            CurrentBuilding.Name = "" + ModeManager.ResourceBuildType + "_" + CurrentBuilding.SelfIndex;
            CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/Economy/Salmon/SalmonCottage.tres");
        }
        if (BuildingType == Constants.HOUSE){
            ModeManager.HouseCount++;
            ModeManager.BuildingCount++;
            Instantiate_building(buildingPath);
            CurrentBuilding.SelfIndex = ModeManager.HouseCount;
            CurrentBuilding.Name = "" + ModeManager.ResourceBuildType + "_" + CurrentBuilding.SelfIndex;
            CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/House/House.tres");
        }
        CurrentBuilding.InputPickable = false;
        ModeManager.CurrentLevel.AddChild(CurrentBuilding);
        MousePosition = ModeManager.CurrentLevel.GetGlobalMousePosition();
        CurrentBuilding.GlobalPosition = MousePosition;
        CurrentConstructors = GetNode<SimulationMode>("../SimulationMode").SelectedAllies;
    }

    public override void Update() {
        Move_preview();
        Validate_position();
        if (Input.IsActionJustPressed("LeftClick")) {
            Confirm_building();
        }
        if (Input.IsActionJustPressed("RightClick")) {
            Cancel_building();
        }
    }

    private void Cancel_building() {
        ModeManager.FightersTowerCount--;
        ModeManager.BuildingCount--;
        CurrentBuilding.QueueFree();
        EmitSignal("ModeTransition", "SimulationMode", "", "");
    }

    private void Confirm_building() {
        if (!IsOverlappingBuildings) {
            CurrentBuilding.RebakeAddBuilding();
            CurrentBuilding.Rebake();
            CurrentBuilding.InputPickable = true;
            EmitSignal("ConstructionStarted", CurrentBuilding);
            BuildCompleted += When_building_completed;
            EmitSignal("ModeTransition", "SimulationMode", "", "");
        }
    }

    private void Validate_position() {
        Area2D gridArea = CurrentBuilding.GetNode<Area2D>("GridArea");
        Godot.Collections.Array<Area2D> overlappingAreas = gridArea.GetOverlappingAreas();
        foreach (var area in overlappingAreas) {
            if (area.Name == "GridArea") {
                IsOverlappingBuildings = true;
                break;
            }
        }
        if(overlappingAreas.Count == 0) {
            IsOverlappingBuildings = false;    
        }
        if (IsOverlappingBuildings) {
            CurrentBuilding.Modulate = CurrentBuilding.ErrorColor;
        } else {
            CurrentBuilding.Modulate = CurrentBuilding.OkColor;
        }
    }

    private void Move_preview() {
        MousePosition = ModeManager.CurrentLevel.GetGlobalMousePosition();
        float xDifference = MousePosition.X - CurrentBuilding.GlobalPosition.X;
        float yDifference = MousePosition.Y - CurrentBuilding.GlobalPosition.Y;
        float newX = 0;
        float newY = 0;
        if (xDifference > (TileSizeX / 2)) {
            newX = (TileSizeX / 2);

        } else if (xDifference < (TileSizeX / 2) * -1) {
            newX = (TileSizeX / 2) * -1;
        }
        if (yDifference > (TileSizeY / 2)) {
            newY = (TileSizeY / 2);
        } else if (yDifference < (TileSizeY / 2) * -1) {
            newY = (TileSizeY / 2) * -1;
        }
        CurrentBuilding.GlobalPosition = new Vector2(
            CurrentBuilding.GlobalPosition.X + newX,
            CurrentBuilding.GlobalPosition.Y + newY
        );

    }

    private void Instantiate_building(String buildingPath) {
        PackedScene buildingScene = GD.Load<PackedScene>(buildingPath);
        CurrentBuilding = (Building)buildingScene.Instantiate();
    }

    public void When_building_completed(Building building){
        building.Modulate = building.RegularColor;
        building.CurrentBuilders.Clear();
    }
}
