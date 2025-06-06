using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class AllyState : State
{
    public Controller Controller;
    public Ally Ally;

    /// <summary>
    /// Inicializa dados pertinentes a unidades aliadas.
    /// </summary>
    public void InitializeAlly() {
        Ally = (Ally) Unit;
        Controller = GetNode<Controller>("/root/Game/Controller");
        SimulationMode.Singleton.AllyCommand += CommandReceived;
    }
    
    /// <summary>
    /// <param name="coords">Ponto de referência para medir distância</param>
    /// <param name="closestBuildingPosition">Atual construção mais próxima.</param>
    /// <param name="building">Construção a ser comparada com a mais próxima.</param>
    /// Compara a distancia entre duas construções em relação a um ponto específico. Normalmente a posição atual da unidade.
    /// <returns><c>Vector2</c> Coordenadas globais.</returns>
    /// </summary>
    public Vector2 GetClosestBuilding(Vector2 coords, Vector2 closestBuildingPosition, Building building) {
        if (closestBuildingPosition == default) {
            return building.GlobalPosition;
        }

        if (closestBuildingPosition.DistanceSquaredTo(coords) > building.GlobalPosition.DistanceSquaredTo(coords)) {
            return building.GlobalPosition;
        }
        return closestBuildingPosition;
    }

    public virtual void ChooseNextTargetPosition(Vector2 coords) {}

    public override void CommandReceived(Vector2 coords)
    {
        if (!Ally.CurrentlySelected) { return; }

        ChooseNextTargetPosition(coords);
        ChangeState("Move");
    }
}
