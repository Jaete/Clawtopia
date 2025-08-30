using Godot;
using ClawtopiaCs.Scripts.Entities.Characters;

public partial class Build : EconomicState
{
    public override void Enter()
    {
        Ally.ConstructionToBuild = Ally.InteractedBuilding;
        Ally.ConstructionToBuild.CurrentBuilders.Add(Ally);

        PlayBuildAnimation();
    }

    public override void Update(double delta) { }

    public override void Exit()
    {
        Ally.ConstructionToBuild.CurrentBuilders.Remove(Ally);
        Ally.InteractedBuilding = null;
        Ally.ConstructionToBuild = null;
        Ally.AllyIsBuilding = false;
    }

    public override void CommandReceived(Vector2 coords) { }

    public override void NavigationFinished() { }

    private void PlayBuildAnimation()
    {
        float angle = Mathf.RadToDeg(Ally.LastDirection.Angle());

        if (angle <= 90 && angle > -90)
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.BuildingRight], false);
        else
            SpriteHandler.ChangeAnimation(Ally.Sprite, Ally.AnimController.animMap[CharacterAnim.BuildingLeft], false);
    }
}