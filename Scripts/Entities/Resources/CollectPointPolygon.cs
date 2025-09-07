using Godot;

[GlobalClass, Tool]
public partial class CollectPointPolygon : PolygonArea
{
    [Export] private CollectPoint Point { get; set; }
    public override void OnRectChanged()
    {
        if (IsNameEquals(Point.BodyShape.Name))
        {
            Point.Resource.BodyShape = NewPolygon();
        }

        if (IsNameEquals(Point.InteractionShape.Name))
        {
            Point.Resource.Interaction = NewPolygon();
        }

        ResourceSaver.Save(Point.Resource, Point.Resource.ResourcePath);
    }
}
