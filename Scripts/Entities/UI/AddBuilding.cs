using ClawtopiaCs.Scripts.Entities.UI;
using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using ClawtopiaCs.Scripts.Systems.Tooling;
using Godot;

public partial class AddBuilding : UIButton
{

    [Export(PropertyHint.Enum)] public BuildingData BuildingToAdd;

    public UI Ui;

    public override void _Ready()
    {
        base._Ready();
        Ui = GetNode<UI>("/root/Game/UI");
        Pressed += OnPressed;
    }

    public override void OnPressed()
    {
        base.OnPressed();

        Building building = BuildingLoader.Singleton.LoadBuilding(BuildingToAdd);

        foreach (var resource in building.Data.ResourceCosts.Keys)
        {
            if (LevelManager.Singleton.CurrentResources[resource] < building.Data.ResourceCosts[resource])
            {
                //TODO: colocar implementacao de exibir UI indicando que nao tem recurso, voz, etc.
                GD.Print("Not enough resources");
                return;
            }
        }

        ModeManager.Singleton.CurrentMode.EmitSignal(GameMode.SignalName.ModeTransition, GameMode.BUILD_MODE, building);
    }
}