using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class PolygonArea : CollisionPolygon2D
{
    [Signal] public delegate void PolygonChangedEventHandler();

    [Signal] public delegate void PointsChangedEventHandler();

    [Export] private Building _building;

    public override void _Ready()
    {
        base._Ready();
        PolygonChanged += OnRectChanged;
        EmitSignal(SignalName.PolygonChanged);
    }
    public override bool _Set(StringName property, Variant value)
    {
        var result = base._Set(property, value);
        EmitSignal(SignalName.PolygonChanged);
        return result;
    }

    public void OnRectChanged()
	{
        if (!BuildingData.ValidateBuilding(_building, MethodName.OnRectChanged))
        {
            return;
        }

        if (IsNameEquals(_building.GridShape.Name))
        {
            SetGridArea();
        }
        else if (IsNameEquals(_building.InteractionShape.Name))
        {
            SetInteractionShape();
        }
        else if (IsNameEquals(_building.BodyShape.Name))
        {
            SetCollision();
        }
        ResourceSaver.Singleton.Save(_building.Data, _building.Data.ResourcePath);
    }

    private bool IsNameEquals(StringName name)
    {
        return Name == name;
    }


    private ConcavePolygonShape2D NewPolygon()
    {
        return new ConcavePolygonShape2D()
        {
            Segments = Polygon
        };
    }

    private void SetGridArea()
    {
        if (_building.IsRotated)
        {
            _building.Data.Structure.RotatedGridArea = NewPolygon();
        }
        else
        {
            _building.Data.Structure.GridArea = NewPolygon();
        }
    }

    private void SetInteractionShape()
    {
        if (_building.IsRotated)
        {
            _building.Data.Structure.RotatedInteraction = NewPolygon();
        }
        else
        {
            _building.Data.Structure.Interaction = NewPolygon();
        }
    }

    private void SetCollision()
    {
        if (_building.IsRotated)
        {
            _building.Data.Structure.RotatedCollision = NewPolygon();
        }
        else
        {
            _building.Data.Structure.Collision = NewPolygon();
        }
    }
}
