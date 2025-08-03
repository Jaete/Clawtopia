using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;
using Godot.Collections;
using static BuildingData;

public partial class EconomicBehaviour : Resource
{
    public static void SeekResource(Ally ally, ResourceType resourceType)
    {
        Array<CollectPoint> resourceToSeek = Selectors.GetCollectPoints(resourceType);

        foreach (var point in resourceToSeek)
        {
            ally.InteractedResource = (CollectPoint)Selectors.GetClosestObject(
                 ally.GlobalPosition,
                 ally.InteractedResource,
                 point
             );
        }
        ally.Navigation.SetTargetPosition(ally.InteractedResource.GlobalPosition);
        ally.InteractedResource.ResourceType = resourceType;
        ally.InteractedWithBuilding = false;
    }

    public static void DeliverResource(Ally ally)
    {
        var resource = new Dictionary<ResourceType, int>
        {
            { ally.InteractedResource.ResourceType, ally.ResourceCurrentQuantity }
        };

        LevelManager.Singleton.EmitSignal(LevelManager.SignalName.ResourceDelivered, resource);

        ally.ResourceCurrentQuantity = 0;
        ally.Delivering = false;
        ally.Navigation.SetTargetPosition(ally.InteractedResource.GlobalPosition);
    }

    public static void ChooseNextTargetPosition(Ally ally, Vector2 coords)
    {
        Vector2 nextTarget = coords;
        ally.InteractedResource = null;
        ally.InteractedBuilding = null;
        ally.InteractedWithBuilding = SimulationMode.Singleton.BuildingsToInteract.Count > 0;

        if (ally.InteractedWithBuilding)
        {
            ally.InteractedBuilding = SimulationMode.Singleton.BuildingsToInteract[0];
            nextTarget = ally.InteractedBuilding.GlobalPosition;
            nextTarget = GetOffsetCoordsFromPosition(ally.GlobalPosition, nextTarget);
        }
        else
        {
            ResourceType interactedResource = Selectors.GetInteractedResourceType(ally, coords);
            if (interactedResource != ResourceType.None)
            {
                Array<CollectPoint> collectPoints = Selectors.GetCollectPoints(interactedResource);
                nextTarget = Selectors.GetClosestCollectPoint(ally, coords, collectPoints[0]).GlobalPosition;
            }
        }

        ally.Navigation.SetTargetPosition(nextTarget);
    }

    public static Vector2 GetOffsetCoordsFromPosition(Vector2 allyPosition, Vector2 nextTarget)
    {
        //var buildMode = ModeManager.GetNode<BuildMode>("BuildMode");
        //var x = buildingPosition.X - allyPosition.X > 0 ? -1 : 1;
        //var y = buildingPosition.Y - allyPosition.Y > 0 ? 1 : -1;
        //var newCoords = new Vector2(
        //    buildingPosition.X + (buildMode.TileSizeX * x),
        //    buildingPosition.Y + (buildMode.TileSizeY * y)
        //);

        //if (!isPurrlament) return newCoords;

        //newCoords.X += 100 * x;
        //newCoords.Y += 100 * y;

        //@TODO: Ver se isso eh realmente necessario porque talvez nao precise recalcular as coordenadas

        return nextTarget;
    }
}
