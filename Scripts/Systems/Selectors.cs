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
        GD.Print($"Building on front: {buildingInFront.Name}");
        switch (buildingInFront.Data.Type) {
            case Constants.COMMUNE:
                ui.Instantiate_window(Constants.PURRLAMENT_MENU);
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
}