using System;
using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;
using Godot.Collections;

public partial class BuildMode : GameMode
{
    public string BuildingType;

    public Building CurrentBuilding;

    public Array<Ally> CurrentConstructors;
    public bool IsOverlappingBuildings = false;

    public Vector2 MousePosition;

    public int TileSizeX = 64;
    public int TileSizeY = 32;

    public override void Enter()
    {
        String buildingPath = Constants.BUILDING_PATH;

        if (BuildingType == Constants.TOWER)
        {
            ModeManager.FightersTowerCount++;
            ModeManager.BuildingCount++;
            InstantiateBuilding(buildingPath);
            CurrentBuilding.SelfIndex = ModeManager.FightersTowerCount;
            CurrentBuilding.Name = ModeManager.TowerType + "_T1_" + CurrentBuilding.SelfIndex;
            CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/Towers/Fighters/Fighters.tres");
        }

        if (BuildingType == Constants.COMMUNE)
        {
            ModeManager.GreatCommuneCount++;
            ModeManager.BuildingCount++;
            InstantiateBuilding(buildingPath);
            CurrentBuilding.SelfIndex = ModeManager.GreatCommuneCount;
            CurrentBuilding.Name = Constants.COMMUNE;
            CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/GreatCommune/GreatCommune.tres");
        }

        if (BuildingType == Constants.RESOURCE)
        {
            ModeManager.SalmonCottageCount++;
            ModeManager.BuildingCount++;
            InstantiateBuilding(buildingPath);
            CurrentBuilding.SelfIndex = ModeManager.SalmonCottageCount;
            CurrentBuilding.Name = "" + ModeManager.ResourceBuildType + "_" + CurrentBuilding.SelfIndex;
            CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/Economy/Salmon/SalmonCottage.tres");
        }

        if (BuildingType == Constants.HOUSE)
        {
            ModeManager.HouseCount++;
            ModeManager.BuildingCount++;
            InstantiateBuilding(buildingPath);
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

    public override void Update()
    {
        MovePreview();
        ValidatePosition();
    }

    public override void MouseReleased(Vector2 coords)
    {
        ConfirmBuilding();
    }

    public override void MouseRightPressed(Vector2 coords)
    {
        CancelBuilding();
    }

    private void CancelBuilding()
    {
        ModeManager.FightersTowerCount--;
        ModeManager.BuildingCount--;
        CurrentBuilding.QueueFree();
        EmitSignal("ModeTransition", "SimulationMode", "", "");
    }

    private void ConfirmBuilding()
    {
        if (!IsOverlappingBuildings)
        {
            CurrentBuilding.RebakeAddBuilding();
            CurrentBuilding.Rebake();
            CurrentBuilding.InputPickable = true;
            EmitSignal("ConstructionStarted", CurrentBuilding);
            BuildCompleted += WhenBuildingCompleted;
            EmitSignal("ModeTransition", "SimulationMode", "", "");
            LevelManager.EmitSignal(LevelManager.SignalName.ResourceExpended, CurrentBuilding.Data.ResourceCosts);
        }
    }

    private void ValidatePosition()
    {
        Area2D gridArea = CurrentBuilding.GetNode<Area2D>("GridArea");
        Array<Area2D> overlappingAreas = gridArea.GetOverlappingAreas();
        IsOverlappingBuildings = false;

        foreach (var area in overlappingAreas)
        {
            if (area.Name == "GridArea")
            {
                IsOverlappingBuildings = true;
                break;
            }
        }

        Building.ModulateBuilding(
            CurrentBuilding,
            IsOverlappingBuildings ? BuildingInteractionStates.BUILDING_ERROR : BuildingInteractionStates.BUILDING_OK
        );
    }

    private void MovePreview()
    {
        MousePosition = ModeManager.CurrentLevel.GetGlobalMousePosition();
        float xDifference = MousePosition.X - CurrentBuilding.GlobalPosition.X;
        float yDifference = MousePosition.Y - CurrentBuilding.GlobalPosition.Y;
        float newX = 0;
        float newY = 0;

        if (xDifference > (TileSizeX / 2))
        {
            newX = (TileSizeX / 2);
        }
        else if (xDifference < (TileSizeX / 2) * -1)
        {
            newX = (TileSizeX / 2) * -1;
        }

        if (yDifference > (TileSizeY / 2))
        {
            newY = (TileSizeY / 2);
        }
        else if (yDifference < (TileSizeY / 2) * -1)
        {
            newY = (TileSizeY / 2) * -1;
        }

        CurrentBuilding.GlobalPosition = new Vector2(
            CurrentBuilding.GlobalPosition.X + newX,
            CurrentBuilding.GlobalPosition.Y + newY
        );
    }

    private void InstantiateBuilding(String buildingPath)
    {
        PackedScene buildingScene = GD.Load<PackedScene>(buildingPath);
        CurrentBuilding = (Building)buildingScene.Instantiate();
    }

    public void WhenBuildingCompleted(Building building)
    {
        Building.ModulateBuilding(building, BuildingInteractionStates.BUILD_FINISHED);
        building.CurrentBuilders.Clear();
    }
}