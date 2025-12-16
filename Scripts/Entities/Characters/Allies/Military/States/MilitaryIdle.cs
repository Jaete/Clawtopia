using Godot;
using ClawtopiaCs.Scripts.Systems;

public partial class MilitaryIdle : MilitaryState
{
    public override void Enter()
    {
        Ally.Velocity = Vector2.Zero;
        
        // Avisa UI de militar em IDLE
        ResourceCount.Singleton?.OnIdleUnit(1);
    }

    public override void Update(double delta){
    }

    public override void Exit()
    {
        // Avisa UI de militar ocupado
        ResourceCount.Singleton.OnWorkUnit(1);
    }
}
