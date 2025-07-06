using System;
using ClawtopiaCs.Scripts.Entities;
using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;
using Godot.Collections;

public partial class BuildMode : GameMode
{
    public static BuildMode Singleton { get; private set; }

    public string ResourceType;

    public Building CurrentBuilding;

    public BuildingData CurrentBuildingData;

    public Array<Ally> CurrentConstructors;
    public bool IsOverlappingBuildings = false;

    public Vector2 MousePosition;

    public int TileSizeX = 64;
    public int TileSizeY = 32;

    public override void _EnterTree()
    {
        Singleton = this;
    }

    public override void _Ready()
    {
        base._Ready();
        BuildCompleted += WhenBuildingCompleted;
    }

    public override void Enter()
    {
        String buildingPath = Constants.BUILDING_PATH;

        ModeManager.Singleton.BuildingCount++;
        InstantiateBuilding(buildingPath);

        if (CurrentBuilding.Data.Type == Constants.TOWER)
        {
            ModeManager.Singleton.FightersTowerCount++;
            CurrentBuilding.SelfIndex = ModeManager.Singleton.FightersTowerCount;
            CurrentBuilding.Name = ModeManager.Singleton.TowerType + "_T1_" + CurrentBuilding.SelfIndex;
            CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/Towers/Fighters/Fighters.tres");
        }
        if (CurrentBuilding.Data.Type == Constants.RESOURCE)
        {
            switch (CurrentBuilding.Data.ResourceType)
            {
                case Constants.SALMON:
                    ModeManager.Singleton.SalmonCottageCount++;
                    CurrentBuilding.SelfIndex = ModeManager.Singleton.SalmonCottageCount;
                    CurrentBuilding.Name = "" + Constants.FISHERMAN_HOUSE_EXTERNAL_NAME + "_" + CurrentBuilding.SelfIndex;
                    CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/Economy/Salmon/SalmonCottage.tres");
                    break;
                case Constants.CATNIP:
                    ModeManager.Singleton.CatnipFarmCount++;
                    CurrentBuilding.SelfIndex = ModeManager.Singleton.CatnipFarmCount;
                    CurrentBuilding.Name = "" + Constants.DISTILLERY_EXTERNAL_NAME + "_" + CurrentBuilding.SelfIndex;
                    CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/Economy/Catnip/CatnipFarm.tres");
                    break;
                case Constants.SAND:
                    ModeManager.Singleton.SandDepositCount++;
                    CurrentBuilding.SelfIndex = ModeManager.Singleton.SandDepositCount;
                    CurrentBuilding.Name = "" + Constants.SAND_MINE_EXTERNAL_NAME+ "_" + CurrentBuilding.SelfIndex;
                    CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/Economy/Sand/SandMine.tres");
                    break;
            }
        }
        if (CurrentBuilding.Data.Type == Constants.HOUSE)
        {
            ModeManager.Singleton.HouseCount++;
            CurrentBuilding.SelfIndex = ModeManager.Singleton.HouseCount;
            CurrentBuilding.Name = "" + Constants.HOUSE_EXTERNAL_NAME + "_" + CurrentBuilding.SelfIndex;
            CurrentBuilding.Data = GD.Load<BuildingData>("res://Resources/Buildings/House/House.tres");
        }

        CurrentBuilding.InputPickable = false;
        ModeManager.Singleton.CurrentLevel.AddChild(CurrentBuilding);
        MousePosition = ModeManager.Singleton.CurrentLevel.GetGlobalMousePosition();
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

    public override void RotateBuilding()
    {
        Building.RotateBuildingPreview(CurrentBuilding);
    }

    private async void CancelBuilding()
    {
        ModeManager.Singleton.FightersTowerCount--;
        ModeManager.Singleton.BuildingCount--;
        CurrentBuilding.Sounds.Stream = CurrentBuilding.Data.CancelSound;
        CurrentBuilding.Sounds.Play();
        EmitSignal(GameMode.SignalName.ModeTransition, SIMULATION_MODE, "");
        await ToSignal(CurrentBuilding.Sounds, AudioStreamPlayer2D.SignalName.Finished);
        CurrentBuilding.QueueFree();
    }

    private void ConfirmBuilding()
    {
        if (!IsOverlappingBuildings)
        {
            TerrainBaking.Singleton.RebakeAddBuilding(CurrentBuilding);
            TerrainBaking.Singleton.Rebake();
            CurrentBuilding.InputPickable = true;
            EmitSignal(GameMode.SignalName.ConstructionStarted, CurrentBuilding);
            EmitSignal(GameMode.SignalName.ModeTransition, SIMULATION_MODE, "");
            LevelManager.Singleton.EmitSignal(LevelManager.SignalName.ResourceExpended, CurrentBuilding.Data.ResourceCosts);
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

        Modulation.AssignState(
            CurrentBuilding,
            IsOverlappingBuildings ? InteractionStates.ERROR : InteractionStates.OK
        );
    }

    private void MovePreview()
    {
        MousePosition = ModeManager.Singleton.CurrentLevel.GetGlobalMousePosition();
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
        CurrentBuilding.Data = CurrentBuildingData;
    }

    public void WhenBuildingCompleted(Building building)
    {
        Building.PlaceBuilding(building);
    }
}