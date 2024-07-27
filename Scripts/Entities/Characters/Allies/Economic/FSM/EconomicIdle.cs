using System.Runtime.CompilerServices;
using System.Threading;
using Godot;

public partial class EconomicIdle : State
{
    public bool is_current_state;

    public override void Enter(){
        self.Velocity = Vector2.Zero;
        is_current_state = true;
        /* TODO
         Tocar animacao de idle quando houver
        */
    }

    public override void Update(double _delta){
        /*Nao faz nada, afinal e estado Idle*/
        //GD.Print("I'm idle"); // <- Apenas para DEBUG, sera removido depois
    }

    public override void Exit(){
        is_current_state = false;
    }

    public override void When_mouse_right_clicked(Vector2 coords){
        if (!self.currently_selected){ return; }
        Choose_next_target_position(coords);
        Change_state("Move");
    }
}
