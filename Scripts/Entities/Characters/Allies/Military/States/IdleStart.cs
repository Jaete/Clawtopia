using Godot;
using System;

public partial class IdleStart : MilitaryState
{
    public override void _Ready(){
        InitializeAlly();
    }

    public override void Enter(){
        Ally.Sprite.Play("IdleStart");
        Ally.Sprite.AnimationFinished += FinishIdle;
    }

    public override void Update(double _delta){
        if (Ally.Sprite.Animation != "IdleStart"){
            Ally.Sprite.Play("IdleStart");
        }
    }

    public override void Exit(){
        Ally.Sprite.AnimationFinished -= FinishIdle;
    }

    public override void MouseRightClicked(Vector2 coords){
        if (!Ally.CurrentlySelected){ return; }
        ChooseNextTargetPosition(coords);
        ChangeState("Move");
    }

    private void FinishIdle(){
        ChangeState("Idle");
    }
}
