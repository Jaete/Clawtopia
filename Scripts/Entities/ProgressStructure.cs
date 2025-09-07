using System;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class ProgressStructure : Resource
{
    public enum States
    {
        Full = 0,
        Medium,
        Low,
        Empty
    }

    public static Dictionary<int, string> Animations = new()
    {
        { (int) States.Full, "Full" },
        { (int) States.Medium, "Medium" },
        { (int) States.Low, "Low" },
        { (int) States.Empty, "Empty" }
    };

    public enum TextureType {
        Static = 0,
        Animated
    }

    private TextureType _type = TextureType.Static;

    [Export(PropertyHint.Enum)]
    public TextureType Type { 
        get => _type;
        set
        {
            _type = value;
            if (Engine.IsEditorHint())
            {
                NotifyPropertyListChanged();
            }
        }
    }

    public Array<Texture2D> StaticTextures;
    public SpriteFrames AnimatedTextures;

    public override Array<Dictionary> _GetPropertyList()
    {
        var propertyList = new Array<Dictionary>();

        if (Type == TextureType.Static)
        {
            propertyList.Add(new Dictionary
            {
                { "name", "StaticTextures" },
                { "type", (int)Variant.Type.Array },
                { "hint", (int)PropertyHint.ArrayType },
                { "hint_string", $"{Variant.Type.Object:D}/{PropertyHint.ResourceType:D}:Texture2D" }
            });
        }

        if (Type == TextureType.Animated)
        {
            propertyList.Add(new Dictionary
            {
                { "name", "AnimatedTextures" },
                { "type", (int)Variant.Type.Object },
                { "hint", (int)PropertyHint.ResourceType },
                { "hint_string", "SpriteFrames" }
            });
        }

        return propertyList;
    }
}

