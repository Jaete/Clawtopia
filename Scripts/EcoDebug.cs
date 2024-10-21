using Godot;

public partial class EcoDebug : Label
{
	public StateMachine FSM;
	public override void _Ready()
	{
		FSM = GetNode<StateMachine>("../FSM");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta){
		var ally = FSM.CurrentState.Unit as Ally;
		Text = FSM.CurrentState.Name +"\n" + ally.AllyIsBuilding;
	}
}
