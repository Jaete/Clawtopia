using Godot;

public partial class MilitaryMove : MilitaryState
{
    private bool _isChasingEnemy = false;
    private Enemy _targetEnemy;
    public override void Enter(){
    }

    public override void Update(double delta){
        if (_isChasingEnemy){
            Ally.Navigation.SetTargetPosition(_targetEnemy.GlobalPosition);
        }
        Move();
    }
    public override void Exit(){
    }

    public override void MouseRightClicked(Vector2 coords){
        if (!Ally.CurrentlySelected){ return; }
        Choose_next_target_position_MILITARY(coords);
    }
    public override void NavigationFinished(){
        ChangeState("Idle");
    }
    public void When_sight_entered(Node2D body){
        if (body is Enemy enemy){
            _targetEnemy = enemy;
            _isChasingEnemy = true;
        }
    }
}
