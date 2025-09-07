using Godot;
using System;

[GlobalClass]
public partial class CollectableLoaderSettings : ToolSettings
{
    [Export(PropertyHint.File, "*.tres")] public PackedScene BaseCollectable;
}
