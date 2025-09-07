using System;
using ClawtopiaCs.Scripts.Systems.Tooling;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class Collectable : Resource
{
    [ExportSubgroup("Game data")]
    [Export] public string Name { get; set; }
    [Export] public string Description { get; set; }
    [Export] public string CollectorBuildingName { get; set; }

    [ExportSubgroup("Structure")]
    [Export] public ConcavePolygonShape2D BodyShape { get; set; }
    [Export] public ConcavePolygonShape2D Interaction { get; set; }
    [Export] public Texture2D HoverCursor { get; set; }

    [ExportSubgroup("Visuals")]
    [Export] public ProgressStructure ProgressStructure { get; set; }

    private SpriteFrames _collectorAnimations;
    [Export]
    public SpriteFrames CollectorAnimations
    {
        get => _collectorAnimations;
        set
        {
            _collectorAnimations = value;
            if (Engine.IsEditorHint())
            {
                CallDeferred(MethodName.NotifyPropertyListChanged);
            }
        }
    }
    [Export(PropertyHint.Enum)] public string CollectorAnimationLeft { get; set; }
    [Export(PropertyHint.Enum)] public string CollectorAnimationRight { get; set; }

    public override void _ValidateProperty(Dictionary property)
    {
        var propertyName = property["name"].AsStringName();
        if (propertyName != PropertyName.CollectorAnimationLeft && propertyName != PropertyName.CollectorAnimationRight || CollectorAnimations is null)
        {
            base._ValidateProperty(property);
            return;
        }

        var animations = CollectorAnimations.GetAnimationNames();
        if (animations.Length.Equals(0))
        {
            base._ValidateProperty(property);
            return;
        }

        var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
        property["hint_string"] = string.Join(",", animations);
    }
}
