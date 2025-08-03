using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Systems.Tooling;
using Godot;
using Godot.Collections;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime;

[GlobalClass, Tool]
public partial class BuildingEditor : Node2D
{

    [ExportGroup("Settings")]

    private BuildingEditorSettings _settings;
    [Export] public BuildingEditorSettings Settings {
        get => _settings;
        set {
            _settings = value;
            if (!_settings.IsConnected(BuildingList.SignalName.BuildingListChanged, Callable.From(OnBuildingsListChanged)))
            {
                _settings.Buildings.BuildingListChanged += OnBuildingsListChanged;
            }
            CallDeferred(MethodName.NotifyPropertyListChanged);
        }
    }
    private void OnBuildingsListChanged()
    {
        CallDeferred(MethodName.NotifyPropertyListChanged);
    }


    [ExportGroup("New Building")]
    [Export] public string BuildingName;
    [Export] public BuildingData NewBuildingData;
    [ExportToolButton("Add new building", Icon = "Save")]
    public Callable SetBuildings => Callable.From(() =>
    {
        if (!Engine.IsEditorHint()) { return; }
        if (!Validation.ValidateNewBuilding(BuildingName, NewBuildingData)) { return; }

        string buildingsPath = $"{Settings.ResourceDirectory}/Buildings";
        string buildingPath = $"{buildingsPath}/{BuildingName}";
        string buildingDataPath = $"{buildingPath}/{BuildingName}.tres";
        string buildingListPath = $"{buildingsPath}/BuildingList.tres";
        string buildingStructurePath = $"{buildingPath}/Structure.tres";

        if (!Filesystem.DirectoryExists(buildingPath))
        {
            Filesystem.CreateDir(buildingsPath, BuildingName);
        }

        BuildingList buildings = ResourceLoader.Exists(buildingListPath)
            ? GD.Load<BuildingList>(buildingListPath)
            : new BuildingList();

        if (BuildingExists(BuildingName))
        {
            EditorUI.PopupAccept(BuildingName + " already exists on list.");
            return;
        }

        BuildingData newBuildingData = ResourceLoader.Exists(buildingDataPath)
            ? GD.Load<BuildingData>(buildingDataPath)
            : NewBuildingData;

        SaveBuildingData(newBuildingData, buildingDataPath, buildingStructurePath);

        buildings.List.Add(ResourceLoader.Load<BuildingData>(buildingDataPath));

        ResourceSaver.Save(buildings, buildingListPath);

        Settings.Buildings = buildings;
        NotifyPropertyListChanged();

        Open(newBuildingData.Name);
        CleanNewBuildingEditorData();
    });

    [ExportGroup("Load Building")]
    [Export(PropertyHint.EnumSuggestion)] public string BuildingToLoad { get; set; }
    [ExportToolButton("Load building", Icon = "Load")]
    public Callable LoadBuildings => Callable.From(() =>
    {
        Open(BuildingToLoad);
    });


    [ExportGroup("Delete building")]
    [Export(PropertyHint.EnumSuggestion)] public string BuildingToDelete { get; set; }
    [ExportToolButton("Delete building", Icon = "Delete")]
    public Callable DeleteBuilding => Callable.From(() =>
    {
        Delete(BuildingToDelete);
    });

    private void Delete(string buildingToDelete)
    {
        if (!Engine.IsEditorHint()) { return; }
        string buildingsPath = $"{Settings.ResourceDirectory}/Buildings";
        string buildingPath = $"{buildingsPath}/{buildingToDelete}";
        if (!Filesystem.DirectoryExists(buildingPath))
        {
            EditorUI.PopupAccept($"Directory {buildingPath} does not exist. Check your Tool Settings for the actual Resources folder.");
            return;
        }
        Error status = OS.MoveToTrash(buildingPath);
        if (status == Error.Ok)
        {
            EditorUI.PopupAccept($"Building {buildingToDelete} moved to trash successfully. You can recover the file if this was by mistake.");
            return;
        }

        EditorUI.PopupAccept($"Failed to delete building {buildingToDelete}. Error: {status}.");


    }

    public static BuildingData LoadBuildingData(string buildingName, BuildingList buildings)
    {
        if (buildings == null)
        {
            GD.PushError("Building List is not set.");
            return null;
        }

        var building = buildings.List
            .FirstOrDefault(b => b.Name.Equals(buildingName, StringComparison.OrdinalIgnoreCase));

        if (building == null)
        {
            GD.PushError("Building not found on the building List.");
            return null;
        }

        return building;
    }

    public static void ReloadBuilding(Building building)
    {
        building.NotifyPropertyListChanged();

        if (BuildingData.ValidateBuilding(building, MethodName.ReloadBuilding))
        {
            building.Initialize();
            building.QueueRedraw();
        }
    }

    private void SaveBuildingData(BuildingData newBuildingData, string buildingDataPath, string buildingStructurePath)
    {
        newBuildingData.Name = BuildingName;

        newBuildingData.ResourceSceneUniqueId = Resource.GenerateSceneUniqueId();
        newBuildingData.Structure.ResourceSceneUniqueId = Resource.GenerateSceneUniqueId();

        ResourceSaver.Save(newBuildingData.Structure, buildingStructurePath);

        newBuildingData.Structure = ResourceLoader.Load<BuildingStructure>(buildingStructurePath);

        ResourceSaver.Save(newBuildingData, buildingDataPath);
    }

    private void Open(string name)
    {
        if (!Engine.IsEditorHint()) { return; }
        var data = Settings.Buildings.List.FirstOrDefault(b => b.Name == name);

        var newBuilding = LoadBuilding(data);

        if (newBuilding == null)
        {
            GD.PushError("Failed to load new building: " + name);
            return;
        }

        CallDeferred(MethodName.EditNewBuildingScene, newBuilding);
    }

    private void EditNewBuildingScene(Building newBuilding)
    {
        if (!Engine.IsEditorHint()) { return; }
        newBuilding.Type = newBuilding.Data.Name;
        EditorInterface.Singleton.GetEditorViewport2D().GlobalCanvasTransform = new Transform2D(0, GlobalPosition);
        CallDeferred(MethodName.SetNewBuildingData, newBuilding);
    }

    private void SetNewBuildingData(Building newBuilding)
    {
        if (!Engine.IsEditorHint()) { return; }
        if (GetChildren().Count > 0)
        {
            GD.PushWarning("Building editor already has a child. Removing the existing one.");
            GetChildren()[0].QueueFree();
            newBuilding.Name = newBuilding.Data.Name;
        }
        AddChild(newBuilding);
        newBuilding.Owner = this;
        SetEditableInstance(newBuilding, true);
        EditorInterface.Singleton.EditNode(this);
        SetDisplayFolded(false);
        EditorInterface.Singleton.EditNode(newBuilding);
        CallDeferred(MethodName.ReloadBuilding, newBuilding);
    }

    public Building LoadBuilding(BuildingData buildingData)
    {
        if (buildingData == null) return null;
        Building building = Settings.BaseBuilding.Instantiate<Building>();
        building.Data = buildingData;
        building.Initialize();
            return building;
    }

    private void CleanNewBuildingEditorData()
    {
        BuildingName = null;
        NewBuildingData = null;
    }

    private bool BuildingExists(string buildingName)
    {
        return Settings.Buildings.List.Any(b => b.Name == buildingName);
    }

    public override void _ValidateProperty(Dictionary property)
    {
        if (!Engine.IsEditorHint())
        {
            base._ValidateProperty(property);
            return;
        };

        if (property["name"].AsStringName() == PropertyName.BuildingToLoad)
        {
            var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
            property["hint_string"] = string.Join(",", BuildingLoader.GetBuildingNames(Settings.Buildings));
        }

        if (property["name"].AsStringName() == PropertyName.BuildingToDelete)
        {
            var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
            property["hint_string"] = string.Join(",", Filesystem.GetDirectories($"{Settings.ResourceDirectory}/Buildings"));
        }

        base._ValidateProperty(property);
    }

    public override void _ExitTree()
    {
        if (_settings?.Buildings != null)
        {
            _settings.Buildings.BuildingListChanged -= OnBuildingsListChanged;
        }
        base._ExitTree();
    }
}
