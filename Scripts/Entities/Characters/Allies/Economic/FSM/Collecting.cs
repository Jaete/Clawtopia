using Godot;
using Godot.Collections;
using System;

public partial class Collecting : AllyState
{
	private string CATNIP = "catnip";
	private string SALMON = "salmon";
	private string SAND = "sand";
	public string current_collectable;
	public override void Enter(){
		switch (ally.interacted_building.data.NAME){
			
		}
		current_collectable = CATNIP;
	}

	public override void Update(double _delta) {
		GD.Print("Collecting");
	}

	public override void Exit(){
        
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
