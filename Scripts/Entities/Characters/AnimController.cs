using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class AnimController : Resource 
{
    private SpriteFrames _animations;
    [Export] public SpriteFrames Animations
    {
        get => _animations;
        set
        {
            _animations = value;
            if (Engine.IsEditorHint())
            {
                CallDeferred(MethodName.NotifyPropertyListChanged);
            }
        }
    }

    [Export(PropertyHint.Enum)]
    public string AnimationLeft { get; set; }
    [Export(PropertyHint.Enum)]
    public string AnimationRight { get; set; }
    [Export(PropertyHint.Enum)]
    public string AnimationUpLeft { get; set; }
    [Export(PropertyHint.Enum)]
    public string AnimationUpRight { get; set; }
    [Export(PropertyHint.Enum)]
    public string AnimationDownLeft { get; set; }
    [Export(PropertyHint.Enum)]
    public string AnimationDownRight { get; set; }
    [Export(PropertyHint.Enum)]
    public string AnimationDown { get; set; }
    [Export(PropertyHint.Enum)]
    public string AnimationUp { get; set; }


    public override void _ValidateProperty(Dictionary property)
    {
        if (property["name"].AsStringName() == PropertyName.Animations || Animations is null || Animations.GetAnimationNames().Length.Equals(0))
        {
            base._ValidateProperty(property);
            return;
        }

        var animations = Animations.GetAnimationNames();
        var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
        property["hint_string"] = string.Join(",", animations);
    }
}