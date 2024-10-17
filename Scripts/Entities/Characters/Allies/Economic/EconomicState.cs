using Godot;

public partial class EconomicState : AllyState
{

    private bool IsBuilding;
    
    public override void _Ready(){
        InitializeUnit();
        InitializeAlly();
        BuildMode = ModeManager.GetNode<BuildMode>("BuildMode");
        BuildMode.ConstructionStarted += BuildStarted;
        BuildMode.BuildCompleted += BuildCompleted;
    }
    private void BuildStarted(Building building){
        if (!Ally.CurrentlySelected){ return; }
        Ally.AllyIsBuilding = true;
        Ally.ConstructionToBuild = building;
        Ally.Navigation.SetTargetPosition(Ally.ConstructionToBuild.GlobalPosition);
        ChangeState("Move");
    }
    private void BuildCompleted(Building building){
        if (BuildMode.CurrentConstructors.Contains(Ally)){
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
    public Vector2 GetClosestResourceBuilding(Vector2 coords, string resource){
        Vector2 closestBuildingPosition = default;
        switch (resource){
            case Constants.SALMON:
                foreach (var building in Ally.LevelManager.SalmonBuildings){
                    closestBuildingPosition = GetClosestBuilding(coords, closestBuildingPosition, building);
                }
                break;
            case Constants.CATNIP:
                foreach (var building in Ally.LevelManager.CatnipBuildings){ 
                    closestBuildingPosition = GetClosestBuilding(coords, closestBuildingPosition, building);
                }
                break;
            case Constants.SAND:
                foreach (var building in Ally.LevelManager.SandBuildings){ 
                    closestBuildingPosition = GetClosestBuilding(coords, closestBuildingPosition, building);
                }
                break;
        }
        return closestBuildingPosition;
    }
    public void ChooseNextTargetPosition(Vector2 coords){
        Ally.InteractedWithBuilding = SimulationMode.BuildingToInteract != null;
        if (Ally.InteractedWithBuilding){
            Ally.Navigation.SetTargetPosition(
                RecalculateCoords(
                    Ally.GlobalPosition,
                    SimulationMode.BuildingToInteract.GlobalPosition
                ));
            Ally.InteractedBuilding = SimulationMode.BuildingToInteract;
        }else if (IsInteractingWithWaterAt(coords)){
            Ally.InteractedResource = Constants.SALMON; // VARIAVEL NO ALIADO BASE PARA REFERENCIA
            Ally.InteractedBuilding = null;
            Ally.CurrentResourceLastPosition = GetClosestWaterCoord();
            Ally.Navigation.SetTargetPosition(Ally.CurrentResourceLastPosition);
        }else{
            Ally.InteractedResource = null;
            Ally.InteractedBuilding = null;
            Ally.Navigation.SetTargetPosition(coords);
        }
    }
    
    /// <summary>
    /// Verifica se esta interagindo com um tile de agua, usando DataLayer no Tileset.
    /// Pressupoe-se que nao esta interagindo com uma estrutura ou inimigo, pois a verificacao
    /// deles é feita primeiro.
    /// </summary>
    /// <param name="coords">A posicao global do clique.</param>
    /// <returns> <c>bool</c> Se é agua ou não.</returns>
    public bool IsInteractingWithWaterAt(Vector2 coords){
        var mapCoords = SimulationMode.TileMap.LocalToMap(coords);
        var data = SimulationMode.TileMap.GetCellTileData(0, mapCoords, true);
        return (bool) data.GetCustomData("is_water");
    }
    
    /// <summary>
    /// Procura todos os tiles de agua no cenario e retorna as coordenadas globais do
    /// tile mais proximo.
    /// </summary>
    /// <returns> <c>Vector2</c> Coordenadas globais do tile mais proximo.</returns>
    public Vector2 GetClosestWaterCoord(){
        var tilemap = SimulationMode.TileMap;
        var waterTiles = tilemap.GetUsedCellsById(1);
        Vector2 closestTile = default;
        foreach (var tile in waterTiles){
            var localTilePos = tilemap.MapToLocal(tile);
            var globalTilePos = tilemap.ToGlobal(localTilePos);
            if (closestTile == default){
                closestTile = globalTilePos;
            }else if (Ally.GlobalPosition.DistanceSquaredTo(closestTile) > Ally.GlobalPosition.DistanceSquaredTo(globalTilePos)){
                closestTile = globalTilePos;
            }
        }
        return closestTile;
    }
}