using Godot;

[GlobalClass, Tool]
public partial class CollectPointPolygon : PolygonArea
{
    [Export] private CollectPoint Point { get; set; }
    public override void OnRectChanged()
    {
        if (IsNameEquals(Point.BodyShape.Name))
        {
            GD.Print("Setting BodyShape for CollectPoint: ", Point.Name);
            Point.Resource.BodyShape = NewPolygon();
        }

        if (IsNameEquals(Point.InteractionShape.Name))
        {
            GD.Print("Setting InteractionShape for CollectPoint: ", Point.Name);
            Point.Resource.Interaction = NewPolygon();
        }

        GD.Print("Saving: ", Point.Resource.BodyShape, "\n", Point.Resource.Interaction, "\n", Point.Resource.ResourcePath);
        ResourceSaver.Save(Point.Resource, Point.Resource.ResourcePath);
    }
}
