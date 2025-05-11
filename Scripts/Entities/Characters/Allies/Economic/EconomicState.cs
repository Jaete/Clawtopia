using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;
using Godot.Collections;

public partial class EconomicState : AllyState
{

    private bool IsBuilding;

    public override void _Ready()
    {
        InitializeUnit();
        InitializeAlly();
        BuildMode.Singleton.ConstructionStarted += BuildStarted;
        BuildMode.Singleton.BuildCompleted += BuildCompleted;
    }

    private void BuildStarted(Building building)
    {
        if (!Ally.CurrentlySelected)
        {
            return;
        }

        Ally.AllyIsBuilding = true;
        Ally.InteractedBuilding = building;
        Ally.Navigation.SetTargetPosition(Ally.InteractedBuilding.GlobalPosition);
        ChangeState("Move");
    }

    private void BuildCompleted(Building building)
    {
        if (building.CurrentBuilders.Contains(Ally))
        {
            Ally.AllyIsBuilding = false;
            ChangeState("Idle");
        }
    }

    /// <summary>
    /// Procura todos os nodes de construcoes do recurso especifico no cenario
    /// e retorna as coordenadas globais da construcao mais proxima.
    ///
    /// Utiliza-se da funcao <c>GetClosestBuilding</c> para evitar repeticao
    /// de codigo.
    /// </summary>
    /// <returns> <c>Vector2</c> Coordenadas globais da construcao mais proxima</returns>
    public Vector2 GetClosestResourceBuilding(Vector2 coords, string resource)
    {
        Vector2 closestBuildingPosition = default;
        Array<Building> buildingsToSearch = new();

        switch (resource)
        {
            case Constants.SALMON:
                buildingsToSearch = Ally.LevelManager.SalmonBuildings;
                break;
            case Constants.CATNIP:
                buildingsToSearch = Ally.LevelManager.CatnipBuildings;
                break;
            case Constants.SAND:
                buildingsToSearch = Ally.LevelManager.SandBuildings;
                break;
        }
        if (buildingsToSearch.Count > 0) {
            foreach (var building in buildingsToSearch)
            {
                closestBuildingPosition = Selectors.GetClosestObject(coords, closestBuildingPosition, building);
            }
        }
        else
        {
            closestBuildingPosition = RecalculateCoords(
                Ally.GlobalPosition,
                Ally.LevelManager.Purrlament.GlobalPosition,
                true
            );
        }
        return closestBuildingPosition;
    }

    public override void ChooseNextTargetPosition(Vector2 coords)
    {
        Vector2 nextTarget = coords;
        Ally.InteractedResource = null;
        Ally.InteractedBuilding = null;
        Ally.InteractedWithBuilding = SimulationMode.Singleton.BuildingsToInteract.Count > 0;

        if (Ally.InteractedWithBuilding)
        {
            nextTarget = RecalculateCoords(Ally.GlobalPosition,
                SimulationMode.Singleton.BuildingsToInteract[0].GlobalPosition);

            Ally.InteractedBuilding = SimulationMode.Singleton.BuildingsToInteract[0];
        }
        else
        {
            if (Selectors.IsInteractingWithResource(Ally, coords, Constants.SALMON))
            {
                nextTarget = GetClosestResourcePosition(coords, Constants.SALMON);
            }
            else if (Selectors.IsInteractingWithResource(Ally, coords, Constants.CATNIP))
            {
                nextTarget = GetClosestResourcePosition(coords, Constants.CATNIP);
            }
            else if (Selectors.IsInteractingWithResource(Ally, coords, Constants.SAND))
            {
                nextTarget = GetClosestResourcePosition(coords, Constants.SAND);
            }
        }

        Ally.Navigation.SetTargetPosition(nextTarget);
    }

    public Vector2 GetClosestResourcePosition(Vector2 coords, string resource)
    {
        Ally.InteractedResource = resource;
        foreach (var point in LevelManager.Singleton.CollectPoints)
        {
            Ally.CurrentResourceLastPosition = Selectors.GetClosestObject(
                coords,
                Ally.CurrentResourceLastPosition,
                point
            );
        }
        return Ally.CurrentResourceLastPosition;
    }
}