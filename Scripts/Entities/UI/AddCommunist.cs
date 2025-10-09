using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot.Collections;
using Godot;
using ClawtopiaCs.Scripts.Entities.UI;

public partial class AddCommunist : UIButton
{
    private Vector2 _communistPosition = new Vector2(-48.0f, 24.0f);
    private Vector2 _communistPositionRotated = new Vector2(60.0f, 20.0f);

    private PackedScene _scene = GD.Load<PackedScene>("res://TSCN/Entities/Characters/Allies/Economic/Economic.tscn");

    [Export] public PackedScene Communist;
    public SceneTreeTimer ResourceSpawnTimer;
    [Export] public float SpawnTimer = OS.IsDebugBuild() ? 1 : 20;
    public UI Ui;

    public override void _Ready()
    {
        base._Ready();
        Ui = GetNode<UI>("/root/Game/UI");
        MouseEntered += UI.EnterUIMode;
        MouseExited += UI.EnterUIMode;
        Pressed += OnPressed;
    }

    public override void OnPressed()
    {
        base.OnPressed();
        SpawnCommunist();
    }

    private void SpawnCommunist()
    {
        var communist = _scene.Instantiate<Ally>();
        var buildingMenu = UI.Singleton.CurrentWindow as BuildingMenu;
        var houseNode = (Node2D)InstanceFromId(buildingMenu.BuildingLevelID);

        //INICIA SPAWN DOS GATOS CAMPONESES
        ResourceSpawnTimer = GetTree().CreateTimer(SpawnTimer);

        foreach (var cost in communist.Attributes.ResourceCosts)
        {
            if (LevelManager.Singleton.CurrentResources[cost.Key] < communist.Attributes.ResourceCosts[cost.Key])
            {
                //TODO: colocar implementacao de exibir UI indicando que nao tem recurso, voz, etc.
                GD.Print("QUANTIDADE INSUFICIENTE DE RECURSO");
                return;
            }
        }

        LevelManager.Singleton.EmitSignal(LevelManager.SignalName.ResourceExpended, communist.Attributes.ResourceCosts);
        ResourceSpawnTimer.Timeout += delegate
        {
            if (houseNode is Building house)
            {
                if (house.IsRotated)
                {
                    communist.GlobalPosition = houseNode.GlobalPosition + _communistPositionRotated;
                }
                else
                {
                    communist.GlobalPosition = houseNode.GlobalPosition + _communistPosition;
                }
                ModeManager.Singleton.CurrentLevel.AddChild(communist);
            }

            // Avisa a UI que nasceu um novo gato
            ResourceCount.Singleton.OnUnitSpawned(CounterNames.ECONOMIC, 1);
        };
    }
}