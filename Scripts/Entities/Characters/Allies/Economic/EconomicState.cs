using System.Linq;
using ClawtopiaCs.Scripts.Systems;
using Godot;
using Godot.Collections;
using static BuildingData;

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

    public override void ChooseNextTargetPosition(Vector2 coords)
    {
        EconomicBehaviour.ChooseNextTargetPosition(Ally, coords);
    }

    /// <summary>
    /// Procura todos os nodes de construcoes do recurso especifico no cenario
    /// e retorna as coordenadas globais da construcao mais proxima.
    ///
    /// Utiliza-se da funcao <c>GetClosestObject</c> para evitar repeticao
    /// de codigo.
    /// </summary>
    /// <returns> <c>Building</c> Construcao mais proxima</returns>
    public static Building GetClosestResourceBuilding(Vector2 coords, Collectable resource)
    {
        Building closestBuilding = default;

        Building[] buildingsToSearch = LevelManager.Singleton.CollectorBuildings
            .Where(b => b.ResourceType == resource.Name)
            .ToArray();

        if (buildingsToSearch.Length > 0)
        {
            foreach (var building in buildingsToSearch)
            {
                closestBuilding = (Building)Selectors.GetClosestObject(coords, closestBuilding, building);
            }
        }
        else
        {
            closestBuilding = LevelManager.Singleton.Purrlament;
        }
        return closestBuilding;
    }
}