using Godot;
using System;

public partial class MilitaryMove : State
{
    private bool is_chasing_enemy = false;
    private Enemy target_enemy;
    public override void Enter(){
    }

    public override void Update(double _delta){
        if (is_chasing_enemy){
            Set_target_position(target_enemy.GlobalPosition);
        }
        Move();
        PlayAnimaion(self.Velocity);

    }
    public override void Exit(){
    }

    public override void When_mouse_right_clicked(Vector2 coords){
        if (!self.currently_selected){ return; }
        Choose_next_target_position_MILITARY(coords);
    }
    public override void When_navigation_finished(){
        Change_state("IdleStart");
    }
    public void When_sight_entered(Node2D body){
        if (body is Enemy enemy){
            target_enemy = enemy;
            is_chasing_enemy = true;
        }
    }

    private void PlayAnimaion(Vector2 direction){
        if (direction is { X: > 0, Y: > 0 }){
            self.sprite.Play("Move down");
            self.sprite.FlipH = false;
        }
        if (direction is { X: > 0, Y: < 0 }){
            self.sprite.Play("Move up");
            self.sprite.FlipH = false;
        }
        if (direction is { X: < 0, Y: > 0 }){
            self.sprite.Play("Move down");
            self.sprite.FlipH = true;
        }
        if (direction is { X: < 0, Y: < 0 }){
            self.sprite.Play("Move up");
            self.sprite.FlipH = true;
        }
    }
}
