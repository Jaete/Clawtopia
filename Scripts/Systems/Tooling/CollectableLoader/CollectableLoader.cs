using Godot;
using System;

public partial class CollectableLoader : Node
{
    public static CollectableLoader Singleton { get; set; }
    [ExportCategory("Loader config")]
    [Export(PropertyHint.File, "*.tres")] public CollectableLoaderSettings Settings;
    [ExportCategory("Collectables")]
    [Export]
    public CollectableList Collectables { get; set; }
    public override void _EnterTree()
    {
        Singleton = this;
        if (Engine.IsEditorHint())
        {
            if (Settings == null || string.IsNullOrEmpty(Settings.ResourceDirectory)) { return; }
            string collectableListPath = $"{Settings.ResourceDirectory}/Collectables/CollectableList.tres";
            if (ResourceLoader.Exists(collectableListPath))
            {
                Collectables = ResourceLoader.Load<CollectableList>(collectableListPath);
            }
            else
            {
                Collectables = new CollectableList();
                ResourceSaver.Save(Collectables, collectableListPath);
            }
        }
    }

    public CollectPoint LoadCollectPoint(Collectable resource)
    {
        if (resource == null) return null;
        CollectPoint collectable = Settings.BaseCollectable.Instantiate<CollectPoint>();
        collectable.Resource = resource;
        collectable.Initialize();
        return collectable;
    }
}
