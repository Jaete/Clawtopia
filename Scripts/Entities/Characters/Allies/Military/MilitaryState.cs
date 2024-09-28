using Godot;

public partial class MilitaryState : AllyState
{
    public override void _Ready(){
        InitializeUnit();
        InitializeAlly();
    }
    public void ChooseNextTargetPosition(Vector2 coords){
        Ally.Navigation.SetTargetPosition(coords);
    }
}
