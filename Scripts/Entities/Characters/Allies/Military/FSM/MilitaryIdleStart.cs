using Godot;
using System;

public partial class MilitaryIdleStart : State
{
	public override void _Ready(){
		Initialize();
	}

	public override void Enter(){
		self.sprite.Play("IdleStart");
		self.sprite.AnimationFinished += FinishIdle;
	}

	public override void Update(double _delta){
		if (self.sprite.Animation != "IdleStart"){
			self.sprite.Play("IdleStart");
		}
	}

	public override void Exit(){
		self.sprite.AnimationFinished -= FinishIdle;
	}

	public override void When_mouse_right_clicked(Vector2 coords){
		if (!self.currently_selected){ return; }
		Choose_next_target_position_MILITARY(coords);
		Change_state("Move");
	}

	private void FinishIdle(){
		Change_state("Idle");
	}
}
