using Godot;
using Godot.Collections;
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

    [Export] public Array<Texture2D> StaticTextures;
    [Export] public SpriteFrames AnimatedTextures;
}

