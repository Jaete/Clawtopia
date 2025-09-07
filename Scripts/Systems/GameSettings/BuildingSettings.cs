using Godot;

[GlobalClass, Tool]
public partial class BuildingSettings : Resource
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.Dir)]
    public string ResourceDirectory { get; set; }
    [Export]
    public PackedScene BuildingEditor { get; set; }
    [Export]
    public PackedScene BaseBuilding { get; set; }
    [Export]
    public PackedScene BuildingLoader { get; set; }
    [Export]
    public BuildingList Buildings { get; set; }
}
