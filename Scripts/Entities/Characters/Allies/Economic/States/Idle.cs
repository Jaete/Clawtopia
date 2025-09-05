using Godot;
using ClawtopiaCs.Scripts.Entities.Characters;

public partial class Idle : EconomicState
{
    public override void Enter()
    {
        Ally.Velocity = Vector2.Zero;
        PlayIdleAnimation();
    }

    public override void Exit() { }

    public override void CommandReceived(Vector2 coords)
    {
        if (!Ally.CurrentlySelected)
        {
            return;
        }

        ChooseNextTargetPosition(coords);
        ChangeState("Move");
    }

    private void PlayIdleAnimation()
    {
        float angle = Mathf.RadToDeg(Ally.LastDirection.Angle());

        if (angle <= 90 && angle > -90)
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.IdleRight], false);
        else
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.IdleLeft], false);
    }
}