using ClawtopiaCs.Scripts.Entities.UI;
using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class AddBuilding : UIButton
{

    [Export] public BuildingData Building;
    public LevelManager LevelManager;
    public ModeManager ModeManager;
    public UI Ui;

    public override void _Ready()
    {
        base._Ready();
        Ui = GetNode<UI>("/root/Game/UI");
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
        LevelManager = GetNode<LevelManager>("/root/Game/LevelManager");
        Pressed += OnPressed;
        Building.Initialize();
    }

    public override void OnPressed()
    {
        base.OnPressed();
        foreach (var resource in LevelManager.CurrentResources)
        {
            if (LevelManager.CurrentResources[resource.Key] < Building.ResourceCosts[resource.Key])
            {
                GD.Print("Not enough resources");
                return;
            }
        }

        ModeManager.CurrentMode.EmitSignal(GameMode.SignalName.ModeTransition, GameMode.BUILD_MODE, Building.Type,
            Building.Name);
    }
}