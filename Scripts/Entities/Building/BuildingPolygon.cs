using Godot;

[GlobalClass, Tool]
public partial class BuildingPolygon : PolygonArea
{
    [Export] private Building Building { get; set; }

    public override void OnRectChanged()
    {
        if (!BuildingData.ValidateBuilding(Building, MethodName.OnRectChanged))
        {
            return;
        }
        if (IsNameEquals(Building.GridShape.Name))
        {
            SetGridArea();
        }
        else if (IsNameEquals(Building.InteractionShape.Name))
        {
            SetInteractionShape();
        }
        else if (IsNameEquals(Building.BodyShape.Name))
        {
            SetCollision();
        }
        ResourceSaver.Singleton.Save(Building.Data, Building.Data.ResourcePath);
    }

    private void SetGridArea()
    {
        if (Building.IsRotated)
        {
            Building.Data.Structure.RotatedGridArea = NewPolygon();
        }
        else
        {
            Building.Data.Structure.GridArea = NewPolygon();
        }
    }

    private void SetInteractionShape()
    {
        if (Building.IsRotated)
        {
            Building.Data.Structure.RotatedInteraction = NewPolygon();
        }
        else
        {
            Building.Data.Structure.Interaction = NewPolygon();
        }
    }

    private void SetCollision()
    {
        if (Building.IsRotated)
        {
            Building.Data.Structure.RotatedCollision = NewPolygon();
        }
        else
        {
            Building.Data.Structure.Collision = NewPolygon();
        }
    }
}
