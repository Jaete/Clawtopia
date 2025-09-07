using ClawtopiaCs.Scripts.Systems.Tooling;
using Godot;
using System.Linq;
using Godot.Collections;

[GlobalClass, Tool]
public partial class CollectableEditor : Node2D
{
    [ExportGroup("Settings")]

    private CollectableEditorSettings _settings;
    [Export]
    public CollectableEditorSettings Settings
    {
        get => _settings;
        set
        {
            _settings = value;
            if (!_settings.IsConnected(CollectableList.SignalName.CollectableListChanged, Callable.From(OnCollectablesListChanged)))
            {
                _settings.Collectables.CollectableListChanged += OnCollectablesListChanged;
            }
            CallDeferred(MethodName.NotifyPropertyListChanged);
        }
    }

    private void OnCollectablesListChanged()
    {
        CallDeferred(MethodName.NotifyPropertyListChanged);
    }


    [ExportGroup("New Collectable")]
    [Export] public Collectable NewCollectable;
    [ExportToolButton("Add new collectable", Icon = "Save")]
    public Callable SetCollectables => Callable.From(AddNewCollectable);

    [ExportGroup("Load Collectable")]
    [Export(PropertyHint.EnumSuggestion)] public string CollectableToLoad { get; set; }
    [ExportToolButton("Load Collectable", Icon = "Load")]
    public Callable LoadCollectables => Callable.From(() => { Open(CollectableToLoad); });

    [ExportGroup("Delete Collectable")]
    [Export(PropertyHint.EnumSuggestion)] public string CollectableToDelete { get; set; }
    [ExportToolButton("Delete Collectable", Icon = "Delete")]
    public Callable DeleteCollectables => Callable.From(() => { Delete(CollectableToDelete); });

    [ExportGroup("Save Collectable")]
    [ExportToolButton("Save current Collectable", Icon = "Save")]
    public Callable SaveCurrentBuilding => Callable.From(Save);

    private void Save()
    {
        if (!Engine.IsEditorHint()) { return; }

        CollectPoint collectPoint = (CollectPoint)GetChildren()[0];

        ResourceSaver.Singleton.Save(collectPoint.Resource, collectPoint.Resource.ResourcePath);

        EditorUI.PopupAccept($"CollectPoint {collectPoint.Name} saved successfully.");
    }

    private void AddNewCollectable()
    {
        if (!Engine.IsEditorHint()) { return; }
        if (!Validation.ValidateNewCollectable(NewCollectable)) { return; }

        string collectablesPath = $"{Settings.ResourceDirectory}/Collectables";
        string collectablePath = $"{collectablesPath}/{NewCollectable.Name}";
        string collectableDataPath = $"{collectablePath}/{NewCollectable.Name}.tres";
        string collectableListPath = $"{collectablesPath}/CollectableList.tres";

        if (!Filesystem.DirectoryExists(collectablesPath))
        {
            Filesystem.CreateDir(Settings.ResourceDirectory, "Collectables");
        }

        if (!Filesystem.DirectoryExists(collectablePath))
        {
            Filesystem.CreateDir(collectablesPath, NewCollectable.Name);
        }

        CollectableList collectables = ResourceLoader.Exists(collectableListPath)
            ? GD.Load<CollectableList>(collectableListPath)
            : new CollectableList();

        collectables.Resources ??= new Array<Collectable>();

        if (CollectableExists(NewCollectable.Name))
        {
            EditorUI.PopupAccept(NewCollectable.Name + " already exists on list.");
            return;
        }

        ResourceSaver.Save(NewCollectable, collectableDataPath);
        collectables.Resources.Add(ResourceLoader.Load<Collectable>(collectableDataPath));
        ResourceSaver.Save(collectables, collectableListPath);
        NotifyPropertyListChanged();

        Open(NewCollectable.Name);
        CleanNewCollectableEditorData();
    }


    private void Open(string newCollectable)
    {
        if (!Engine.IsEditorHint()) { return; }

        var collectable = LoadCollectable(newCollectable, Settings.Collectables);
        var collectPoint = LoadCollectPoint(collectable);

        if (collectPoint == null)
        {
            GD.PushError("Failed to load new collectable: " + collectPoint.Name);
            return;
        }

        CallDeferred(MethodName.EditNewCollectPoint, collectPoint);
    }

    private void EditNewCollectPoint(CollectPoint collectPoint)
    {
        if (!Engine.IsEditorHint()) { return; }
        if (GetChildren().Count > 0)
        {
            GD.PushWarning("Collectables Editor already has a child. Removing the existing one.");
            var children = GetChildren();
            foreach (var child in children)
            {
                if (child is CollectPoint existingCollectPoint)
                {
                    existingCollectPoint.QueueFree();
                }
            }
        }
        AddChild(collectPoint);
        collectPoint.Owner = this;
        collectPoint.Name = collectPoint.Resource.Name;
        SetEditableInstance(collectPoint, true);
        EditorInterface.Singleton.EditNode(this);
        SetDisplayFolded(false);
        EditorInterface.Singleton.EditNode(collectPoint);
    }

    private void Delete(string collectableToDelete)
    {
        if (!Engine.IsEditorHint()) { return; }
        string collectablesPath = $"{Settings.ResourceDirectory}/Collectables";
        string collectablePath = $"{collectablesPath}/{collectableToDelete}";
        if (!Filesystem.DirectoryExists(collectablePath))
        {
            EditorUI.PopupAccept($"Directory {collectablePath} does not exist. Check your Tool Settings for the actual Resources folder.");
            return;
        }
        Error status = OS.MoveToTrash(ProjectSettings.GlobalizePath(collectablePath));
        if (status == Error.Ok)
        {
            Settings.Collectables.Resources.Remove(LoadCollectable(collectableToDelete, Settings.Collectables));
            ResourceSaver.Save(Settings.Collectables, Settings.Collectables.ResourcePath);

            EditorUI.PopupAccept($"Collectable {collectableToDelete} moved to trash successfully. You can recover the file if this was by mistake.");
            NotifyPropertyListChanged();
            EditorInterface.Singleton.GetResourceFilesystem().Scan();
            return;
        }

        EditorUI.PopupAccept($"Failed to delete collectable {collectableToDelete}. Error: {status}.");
    }

    private bool CollectableExists(string buildingName)
    {
        if (Settings.Collectables == null)
        {
            return false;
        }

        if (Settings.Collectables.Resources == null)
        {
            return false;
        }

        return Settings.Collectables.Resources.Any(c => c.Name == buildingName);
    }

    public static Collectable LoadCollectable(string name, CollectableList collectables)
    {
        if (collectables == null || collectables.Resources == null || collectables.Resources.Count == 0)
        {
            return null;
        }
        return collectables.Resources.First(r => r.Name == name);
    }

    public CollectPoint LoadCollectPoint(Collectable resource)
    {
        if (resource == null) return null;
        CollectPoint collectable = Settings.BaseCollectable.Instantiate<CollectPoint>();
        collectable.Resource = resource;
        return collectable;
    }

    internal static void ReloadCollectPoint(CollectPoint collectPoint)
    {
        collectPoint.NotifyPropertyListChanged();
        collectPoint.Initialize();
        collectPoint.QueueRedraw();
    }

    internal static void Reset(CollectPoint collectPoint)
    {
        if (IsInstanceValid(collectPoint.AnimatedSprite))
        {
            collectPoint.AnimatedSprite.Free();
        }
        if (IsInstanceValid(collectPoint.StaticSprite))
        {
            collectPoint.StaticSprite.Free();
        }
        if (IsInstanceValid(collectPoint.InteractionShape) && collectPoint.InteractionShape.Polygon != null)
        {
            collectPoint.InteractionShape.Polygon = null;
        }
        if (IsInstanceValid(collectPoint.BodyShape) && collectPoint.BodyShape.Polygon != null)
        {
            collectPoint.BodyShape.Polygon = null;
        }

    }

    protected void CleanNewCollectableEditorData()
    {
        NewCollectable = null;
    }


    public override void _ValidateProperty(Dictionary property)
    {
        if (!Engine.IsEditorHint())
        {
            base._ValidateProperty(property);
            return;
        };

        if (property["name"].AsStringName() == PropertyName.CollectableToLoad)
        {
            var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
            property["hint_string"] = string.Join(",", CollectableList.GetNames(Settings.Collectables.Resources));
        }

        if (property["name"].AsStringName() == PropertyName.CollectableToDelete)
        {
            var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
            property["hint_string"] = string.Join(",", CollectableList.GetNames(Settings.Collectables.Resources));
        }
    }
}

