using Godot;

[GlobalClass, Tool]
public partial class BuildingEditorSettings : ToolSettings
{
    [Export(PropertyHint.File, "*.tscn")] public PackedScene BaseBuilding { get; set; }

    [Export(PropertyHint.File, "*.tres")] public BuildingList Buildings { get; set; }
}
