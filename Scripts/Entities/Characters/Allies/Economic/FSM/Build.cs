using Godot;
using System;

public partial class Build : State
{
	public override void Enter(){
		self.construction_to_build.current_builders.Add(self);
		/*TODO
		 TOCAR ANIMAÇÃO DE BUILD QUANDO TIVER*/
	}
	public override void Update(double _delta){}
	public override void Exit(){
		self.construction_to_build.current_builders.Remove(self);
		self.ally_is_building = false;
	}

	public override void When_mouse_right_clicked(Vector2 coords){
		if (!self.currently_selected){ return; }
		Choose_next_target_position_ECONOMIC(coords);
		Change_state("Move");
	}
	public override void When_navigation_finished(){}
	
}
