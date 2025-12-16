using Godot;
using ClawtopiaCs.Scripts.Entities.Characters;
using ClawtopiaCs.Scripts.Systems;

public partial class Idle : EconomicState
{
    public override void Enter()
    {
        Ally.Velocity = Vector2.Zero;
        PlayIdleAnimation();

        // Avisa UI de aldeão em IDLE
        ResourceCount.Singleton?.OnIdleUnit(1);
    }

    public override void Exit()
    {
        // Avisa UI de aldeão ocupado
        ResourceCount.Singleton.OnWorkUnit(1);
    }

    public override void CommandReceived(Vector2 coords)
    {
        if (!Ally.CurrentlySelected)
        {
            return;
        }

        ChooseNextTargetPosition(coords);
    }

    private void PlayIdleAnimation()
    {
        float angle = Mathf.RadToDeg(Ally.LastDirection.Angle());

        if (angle <= 90 && angle > -90)
            SpriteHandler.ChangeAnimation(Ally.Sprite, AnimationController.AnimationRight);
        else
            SpriteHandler.ChangeAnimation(Ally.Sprite, AnimationController.AnimationLeft);
    }
}