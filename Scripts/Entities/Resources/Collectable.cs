using Godot;

[GlobalClass, Tool]
public partial class Collectable : Resource
{
    [ExportSubgroup("Game data")]
    [Export] public string Name { get; set; }
    [Export] public string Description { get; set; }
    [Export] public BuildingData CollectorBuilding { get; set; }

    [ExportSubgroup("Structure")]
    [Export] public ConcavePolygonShape2D BodyShape { get; set; }
    [Export] public ConcavePolygonShape2D Interaction { get; set; }
    [Export] public Texture2D HoverCursor { get; set; }

    [ExportSubgroup("Visuals")]
    [Export] public ProgressStructure ProgressStructure { get; set; }
}
