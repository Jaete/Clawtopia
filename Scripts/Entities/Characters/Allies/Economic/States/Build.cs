using Godot;

public partial class Build : EconomicState
{
	public override void Enter(){
		Ally.ConstructionToBuild.CurrentBuilders.Add(Ally);
		/*TODO
		 TOCAR ANIMAÇÃO DE BUILD QUANDO TIVER*/
	}
	public override void Update(double delta){}
	public override void Exit(){
		Ally.ConstructionToBuild.CurrentBuilders.Remove(Ally);
		Ally.AllyIsBuilding = false;
	}

	public override void MouseRightClicked(Vector2 coords){
		if (!Ally.CurrentlySelected){ return; }
		ChooseNextTargetPosition(coords);
		ChangeState("Move");
	}
	public override void NavigationFinished(){}
	
}
