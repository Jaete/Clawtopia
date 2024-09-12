using Godot;

public partial class MilitaryIdle : MilitaryState
{
    public override void Enter(){
        Ally.Velocity = Vector2.Zero;
    }

    public override void Update(double delta){
    }

    public override void Exit(){
    }
}
