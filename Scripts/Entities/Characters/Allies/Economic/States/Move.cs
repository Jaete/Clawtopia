using System.Drawing;
using ClawtopiaCs.Scripts.Systems;
using Godot;
using Godot.Collections;
using ClawtopiaCs.Scripts.Entities.Characters;

public partial class Move : EconomicState
{

    protected bool isActive;

    public override void Enter()
    {
        isActive = true;
    }

    public override void Update(double delta)
    {
        
        Move();

        Ally.UpdateDirection(Ally.Velocity);
        PlayMoveAnimation();
    }

    public override void Exit()
    {
        isActive = false;
     }

    public override void CommandReceived(Vector2 coords)
    {
        if (!Ally.CurrentlySelected) { return; }

        ChooseNextTargetPosition(coords);
    }

    public override void NavigationFinished()
    {
        if (Ally.AllyIsBuilding)
        {
            ChangeState("Building");
            return;
        }

        if (Ally.InteractedWithBuilding)
        {
            if (!Ally.InteractedBuilding.IsBuilt)
            {
                Ally.ConstructionToBuild = Ally.InteractedBuilding;
                ChangeState("Building");
                return;
            }

            switch (Ally.InteractedBuilding.Data.Type)
            {
                case Constants.TOWER:
                    ChangeState("Taking_shelter");
                    return;
                case Constants.RESOURCE:
                    EconomicBehaviour.SeekResource(Ally, Ally.InteractedBuilding.Data.ResourceType);
                    return;
            }
        }

        if (Ally.InteractedResource != null && !Ally.Delivering)
        {
            ChangeState("Collecting");
            return;
        }

        if (Ally.Delivering)
        {
            EconomicBehaviour.DeliverResource(Ally);
            return;
        }
        
        ChangeState("Idle");
    }

    private void PlayMoveAnimation()
    {
        // Se o Move ja tiver acabado, ele nao toca
        if (!isActive) return;

        float angle = Mathf.RadToDeg(Ally.LastDirection.Angle());
        
        if (angle >= -22.5 && angle < 22.5)
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.MoveRight], false);
        else if (angle >= 22.5 && angle < 67.5)
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.MoveDownRight], false);
        else if (angle >= 67.5 && angle < 112.5)
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.MoveDown], false);
        else if (angle >= 112.5 && angle < 157.5)
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.MoveDownLeft], false);
        else if (angle >= 157.5 || angle < -157.5)
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.MoveLeft], false);
        else if (angle >= -157.5 && angle < -112.5)
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.MoveUpRight], true);
        else if (angle >= -112.5 && angle < -67.5)
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.MoveUp], false);
        else if (angle >= -67.5 && angle < -22.5)
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.MoveUpRight], false);
    }
}