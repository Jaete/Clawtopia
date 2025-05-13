using ClawtopiaCs.Scripts.Entities.UI;
using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class AddBuilding : UIButton
{

    [Export] public BuildingData Building;
    public UI Ui;

    public override void _Ready()
    {
        base._Ready();
        Ui = GetNode<UI>("/root/Game/UI");
        Pressed += OnPressed;
        Building.Initialize();
    }

    public override void OnPressed()
    {
        base.OnPressed();
        foreach (var resource in LevelManager.Singleton.CurrentResources)
        {
            if (LevelManager.Singleton.CurrentResources[resource.Key] < Building.ResourceCosts[resource.Key])
            {
                //TODO: colocar implementacao de exibir UI indicando que nao tem recurso, voz, etc.
                GD.Print("Not enough resources");
                return;
            }
        }

        ModeManager.Singleton.CurrentMode.EmitSignal(GameMode.SignalName.ModeTransition, GameMode.BUILD_MODE, Building);
    }
}