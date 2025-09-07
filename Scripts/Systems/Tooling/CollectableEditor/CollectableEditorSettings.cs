using Godot;
using System;

[GlobalClass, Tool]
public partial class CollectableEditorSettings : ToolSettings
{
    [Export(PropertyHint.File, "*.tscn")] public PackedScene BaseCollectable { get; set; }
    [Export(PropertyHint.File, "*.tres")] public CollectableList Collectables { get; set; }
}
