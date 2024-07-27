using Godot;
using System;

public partial class ECO_Debug : Label
{
	public StateMachine ally;
	public override void _Ready()
	{
		ally = GetNode<StateMachine>("../FSM");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta){
		Text = ally.current_state.Name +"\n" + ally.current_state.self.ally_is_building;
	}
}
