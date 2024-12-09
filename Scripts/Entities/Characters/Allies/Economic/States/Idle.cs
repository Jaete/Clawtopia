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
        //GD.Print("I'm idle"); // <- Apenas para DEBUG, sera removido depois
    }

    public override void Exit() { }

    public override void MouseRightClicked(Vector2 coords)
    {
        if (!Ally.CurrentlySelected || ModeManager.CurrentMode is BuildMode) {
            return;
        }

        ChooseNextTargetPosition(coords);
        ChangeState("Move");
    }
}