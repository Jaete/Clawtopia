using System.Reflection.Metadata.Ecma335;
using Godot;
using Godot.Collections;

namespace ClawtopiaCs.Scripts.Systems;

public partial class Selectors : Node2D
{
    private static Building SelectTopBuilding(Array<Area2D> overlappingAreas)
    {
        var buildingInFront = new Building();
        if (overlappingAreas.Count == 1) {
            var isBuilding = overlappingAreas[0].HasMeta(new StringName(BuildingData.PROP_ISBUILDING));

            if (isBuilding) {
                buildingInFront = (Building)overlappingAreas[0];
            }
        }
        else {
            foreach (var area in overlappingAreas) {
                var isBuilding = area.GetParent().HasMeta(new StringName(BuildingData.PROP_ISBUILDING));
                var hasHigherY = area.GlobalPosition.Y > buildingInFront.GlobalPosition.Y;
                if (hasHigherY && isBuilding) {
                    buildingInFront = (Building)area.GetParent();
                }
            }
        }

        return buildingInFront;
    }

    public static void SelectSingleBuilding(Array<Area2D> overlappingAreas, UI ui)
    {
        var buildingInFront = SelectTopBuilding(overlappingAreas);
        if (buildingInFront is null) { return; }
        switch (buildingInFront.Data.Type) {
            case Constants.HOUSE:
                ui.Instantiate_window(Constants.HOUSE_MENU, buildingInFront);
                break;
            default:
                ui.Instantiate_window(Constants.BUILDING_MENU, buildingInFront);
                break;
        }
    }

    public static Ally SelectSingleUnit(Array<Area2D> overlappingAreas, UI ui)
    {
        var ally = SelectTopUnit(overlappingAreas);

        if (ally is null) {
            return null;
        }

        ally.CurrentlySelected = true;
        var selectionCircle = ally.GetNode<Line2D>("SelectionCircle");
        selectionCircle.Visible = true;

        return ally;
    }

    public static Array<Ally> SelectMultipleUnits(Array<Area2D> overlappingAreas, Array<Ally> selection = null)
    {
        Array<Ally> selectedAllies = new();
        if (selection != null) {
            selectedAllies = selection;
        }

        foreach (var area in overlappingAreas) {
            if (area.GetParent() is not Ally ally) continue;
            selectedAllies.Add(ally);
            ally.CurrentlySelected = true;
            var selectionCircle = ally.GetNode<Line2D>("SelectionCircle");
            selectionCircle.Visible = true;
        }

        return selectedAllies;
    }

    public static Array<Ally> ClearSelectedAllies(Array<Ally> selectedAllies)
    {
        foreach (var ally in selectedAllies) {
            ally.CurrentlySelected = false;
            var selectionCircle = ally.GetNode<Line2D>("SelectionCircle");
            selectionCircle.Visible = false;
        }

        selectedAllies.Clear();

        return selectedAllies;
    }

    private static Ally SelectTopUnit(Array<Area2D> overlappingAreas)
    {
        var ally = new Ally();
        foreach (var area in overlappingAreas) {
            if (area.GetParent() is not Ally) {
                continue;
            }

            if (area.GlobalPosition.Y > ally.GlobalPosition.Y) {
                ally = (Ally)area.GetParent();
            }
        }

        return ally;
    }

    public static bool IsInteractingWithResource(Ally ally, Vector2 coords, string resource)
    {
        PhysicsDirectSpaceState2D space = ally.GetWorld2D().DirectSpaceState;
        PhysicsPointQueryParameters2D query = new();
        query.Position = coords;
        query.CollisionMask = 1;
        Array<Dictionary> result = space.IntersectPoint(query);

        if (result.Count > 0)
        {
            foreach (var collision in result)
            {
                var collider = collision["collider"].As<GodotObject>();
                switch (resource)
                {
                    case Constants.SALMON:
                        if (collider is SalmonCollectPoint)
                        {
                            return true;
                        }
                        break;
                    case Constants.CATNIP:
                        if (collider is CatnipCollectPoint)
                        {
                            return true;
                        }
                        break;
                    case Constants.SAND:
                        if (collider is SandCollectPoint)
                        {
                            return true;
                        }
                        break;
                }

            }
        }
        return false;
    }

    public static Vector2 GetClosestObject(Vector2 coords, Vector2 currentClosest, Node2D item) {
        // SE É A PRIMEIRA ITERACAO, RETORNA DIRETO
        if (currentClosest == default)
        {
            return item.GlobalPosition;
        }
        // SENAO, COMPARA DISTANCIA
        if (currentClosest.DistanceSquaredTo(coords) > item.GlobalPosition.DistanceSquaredTo(coords))
        {
            return item.GlobalPosition;
        }
        return currentClosest;
    }

    public static Array<CollectPoint> GetCollectPoints(string type) {
        Array<CollectPoint> points = new();

        switch (type) { 
            case Constants.SALMON:
                foreach (var item in LevelManager.Singleton.CollectPoints)
                {
                    if (item is SalmonCollectPoint)
                    {
                        points.Add(item);
                    }
                }
                break;
            case Constants.CATNIP:
                foreach (var item in LevelManager.Singleton.CollectPoints)
                {
                    GD.Print($"Catnip: {item}");
                    if (item is CatnipCollectPoint)
                    {
                        points.Add(item);
                    }
                }
                break;
            case Constants.SAND:
                foreach (var item in LevelManager.Singleton.CollectPoints)
                {
                    if (item is SandCollectPoint)
                    {
                        points.Add(item);
                    }
                }
                break;
        }

        return points;
    }
}