using Godot;

[GlobalClass, Tool]
public partial class BuildingLoaderSettings : ToolSettings
{
    [Export(PropertyHint.File, "*.tscn")] public PackedScene BaseBuilding; 
}
