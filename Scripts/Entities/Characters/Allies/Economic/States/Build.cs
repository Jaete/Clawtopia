using Godot;

public partial class Build : EconomicState
{
    public override void Enter()
    {
        Ally.ConstructionToBuild = Ally.InteractedBuilding;
        Ally.ConstructionToBuild.CurrentBuilders.Add(Ally);
        /*TODO
         TOCAR ANIMAÇÃO DE BUILD QUANDO TIVER*/
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
}