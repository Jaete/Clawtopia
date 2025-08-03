using Godot;

namespace ClawtopiaCs.Scripts.Systems.Interactions;


public partial class StateColors : Node
{
    private static readonly Color error = new("ba000079");
    private static readonly Color hover = new(1.3f, 1.3f, 1.3f);
    private static readonly Color ok = new("2eff3f81");
    private static readonly Color regular = new(1, 1, 1);


    public static Color ErrorColor() { return error; }
    public static Color HoverColor() { return hover; }
    public static Color OkColor() { return ok; }
    public static Color RegularColor() { return regular; }
}
