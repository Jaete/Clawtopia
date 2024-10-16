using Godot;

public partial class MilitaryState : AllyState
{
    public void Choose_next_target_position_MILITARY(Vector2 coords){
        Ally.Navigation.SetTargetPosition(coords);
    }
}
