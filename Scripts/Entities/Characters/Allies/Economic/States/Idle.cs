using Godot;

public partial class Idle : EconomicState
{
    public override void Enter()
    {
        Ally.Velocity = Vector2.Zero;
        /* TODO
         Tocar animacao de idle quando houver
        */
    }

    public override void Update(double delta)
    {
        /*Nao faz nada, afinal e estado Idle*/
    }

    public override void Exit() { }

    public override void CommandReceived(Vector2 coords)
    {
        if (!Ally.CurrentlySelected) {
            return;
        }

        ChooseNextTargetPosition(coords);
        ChangeState("Move");
    }
}