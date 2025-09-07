using Godot;

[GlobalClass, Tool]
public partial class PolygonArea : CollisionPolygon2D
{
    [Signal] public delegate void PolygonChangedEventHandler();
    
    public virtual void OnRectChanged(){}

    public override void _Ready()
    {
        base._Ready();
        PolygonChanged += OnRectChanged;
    }

    public override bool _Set(StringName property, Variant value)
    {
        var result = base._Set(property, value);
        if (Engine.IsEditorHint())
        {
            EmitSignal(SignalName.PolygonChanged);
        }
        return result;
    }

    protected bool IsNameEquals(StringName name)
    {
        return Name == name;
    }

    protected ConcavePolygonShape2D NewPolygon()
    {
        return new ConcavePolygonShape2D()
        {
            Segments = Polygon
        };
    }
}
