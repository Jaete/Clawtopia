using Godot;

public partial class EconomicState : AllyState
{

    private bool IsBuilding;

    public override void _Ready()
    {
        InitializeUnit();
        InitializeAlly();
        BuildMode = ModeManager.GetNode<BuildMode>("BuildMode");
        BuildMode.ConstructionStarted += BuildStarted;
        BuildMode.BuildCompleted += BuildCompleted;
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
        if (BuildMode.CurrentConstructors.Contains(Ally))
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
        bool noResourceBuildings = false;

        switch (resource)
        {
            case Constants.SALMON:
                if(Ally.LevelManager.SalmonBuildings.Count == 0)
                {
                    noResourceBuildings = true;
                    break;
                }
                foreach (var building in Ally.LevelManager.SalmonBuildings)
                {
                    closestBuildingPosition = GetClosestBuilding(coords, closestBuildingPosition, building);
                }
                break;
            case Constants.CATNIP:
                if (Ally.LevelManager.CatnipBuildings.Count == 0)
                {
                    noResourceBuildings = true;
                    break;
                }
                foreach (var building in Ally.LevelManager.CatnipBuildings)
                {
                    closestBuildingPosition = GetClosestBuilding(coords, closestBuildingPosition, building);
                }

                break;
            case Constants.SAND:
                if (Ally.LevelManager.SandBuildings.Count == 0)
                {
                    noResourceBuildings = true;
                    break;
                }
                foreach (var building in Ally.LevelManager.SandBuildings)
                {
                    closestBuildingPosition = GetClosestBuilding(coords, closestBuildingPosition, building);
                }
                break;
        }
        if (noResourceBuildings)
        {
            closestBuildingPosition = RecalculateCoords(
                Ally.GlobalPosition,
                Ally.LevelManager.Purrlament.GlobalPosition,
                true
            );
        }
        return closestBuildingPosition;
    }

    public void ChooseNextTargetPosition(Vector2 coords)
    {
        Vector2 nextTarget = coords;
        Ally.InteractedResource = null;
        Ally.InteractedBuilding = null;
        Ally.InteractedWithBuilding = SimulationMode.BuildingsToInteract.Count > 0;

        if (Ally.InteractedWithBuilding)
        {
            nextTarget = RecalculateCoords(Ally.GlobalPosition,
                SimulationMode.BuildingsToInteract[0].GlobalPosition);

            Ally.InteractedBuilding = SimulationMode.BuildingsToInteract[0];
        }
        else
        {
            if (IsInteractingWithResourceAt(coords, Constants.SALMON))
            {
                Ally.InteractedResource = Constants.SALMON;
                Ally.CurrentResourceLastPosition = GetClosestResourceCoord(Constants.SALMON);
                nextTarget = Ally.CurrentResourceLastPosition;
            }
            else if (IsInteractingWithResourceAt(coords, Constants.CATNIP))
            {
                Ally.InteractedResource = Constants.CATNIP;
                Ally.CurrentResourceLastPosition = GetClosestResourceCoord(Constants.CATNIP);
                nextTarget = Ally.CurrentResourceLastPosition;
            }
            else if (IsInteractingWithResourceAt(coords, Constants.SAND))
            {
                Ally.InteractedResource = Constants.SAND;
                Ally.CurrentResourceLastPosition = GetClosestResourceCoord(Constants.SAND);
                nextTarget = Ally.CurrentResourceLastPosition;
            }
        }

        Ally.Navigation.SetTargetPosition(nextTarget);
    }

    /// <summary>
    /// Verifica se esta interagindo com um tile de agua, usando DataLayer no Tileset.
    /// Pressupoe-se que nao esta interagindo com uma estrutura ou inimigo, pois a verificacao
    /// deles é feita primeiro.
    /// </summary>
    /// <param name="coords">A posicao global do clique.</param>
    /// <returns> <c>bool</c> Se é agua ou não.</returns>
    public bool IsInteractingWithResourceAt(Vector2 coords, string resource)
    {
        Vector2I mapCoords;
        TileData data;

        switch (resource)
        {
            case Constants.SALMON:
                mapCoords = SimulationMode.Water.LocalToMap(SimulationMode.Water.GetLocalMousePosition());
                data = SimulationMode.Water.GetCellTileData(mapCoords);

                return data != null && (bool)data.GetCustomData("isWater");

            case Constants.CATNIP:
                mapCoords = SimulationMode.Bushes.LocalToMap(SimulationMode.Water.GetLocalMousePosition());
                data = SimulationMode.Bushes.GetCellTileData(mapCoords);

                return data != null && (bool)data.GetCustomData("isBush");

            case Constants.SAND:
                mapCoords = SimulationMode.SandDumps.LocalToMap(SimulationMode.SandDumps.GetLocalMousePosition());
                data = SimulationMode.SandDumps.GetCellTileData(mapCoords);

                return data != null && (bool)data.GetCustomData("isSandDump");
        }

        return false;
    }

    /// <summary>
    /// Procura todos os tiles de agua no cenario e retorna as coordenadas globais do
    /// tile mais proximo.
    /// </summary>
    /// <returns> <c>Vector2</c> Coordenadas globais do tile mais proximo.</returns>
    public Vector2 GetClosestResourceCoord(string resource)
    {
        var mapCoords = resource switch
        {
            Constants.SALMON => SimulationMode.Water.GetLocalMousePosition(),
            Constants.CATNIP => SimulationMode.Bushes.GetLocalMousePosition(),
            Constants.SAND => SimulationMode.SandDumps.GetLocalMousePosition(),
            _ => Ally.GlobalPosition
        };

        return mapCoords;
    }
}