using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot.Collections;
using Godot;
using ClawtopiaCs.Scripts.Entities.UI;

public partial class AddCommunist : UIButton
{
    private Vector2 _communistPosition = new Vector2(-145.0f, 85.0f);

    private PackedScene _scene = GD.Load<PackedScene>("res://TSCN/Entities/Characters/Allies/Economic/Economic.tscn");

    [Export] public PackedScene Communist;
    public LevelManager LevelManager;
    public ModeManager ModeManager;
    public SceneTreeTimer ResourceSpawnTimer;
    [Export] public float SpawnTimer = OS.IsDebugBuild() ? 1 : 20;
    public UI Ui;

    public override void _Ready()
    {
        base._Ready();
        Ui = GetNode<UI>("/root/Game/UI");
        MouseEntered += Ui.EnterUiMode;
        MouseExited += Ui.ExitUiMode;
        Pressed += OnPressed;
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");

    }

    public override void OnPressed()
    {
        base.OnPressed();
        SpawnCommunist();
    }

    private void SpawnCommunist()
    {
        
        var addCommunist = _scene.Instantiate<Ally>();
        var purrlamentNode = ModeManager.CurrentLevel.GetNode<Building>(Constants.COMMUNE_EXTERNAL_NAME);
        LevelManager = GetNode<LevelManager>("/root/Game/LevelManager");

        //INICIA SPAWN DOS GATOS CAMPONESES
        ResourceSpawnTimer = GetTree().CreateTimer(SpawnTimer);

        if (LevelManager.CurrentResources[Constants.SALMON] >= 100)
        {
            LevelManager.EmitSignal(LevelManager.SignalName.ResourceExpended, addCommunist.Attributes.ResourceCosts);
            ResourceSpawnTimer.Timeout += delegate
            {
                addCommunist.GlobalPosition = purrlamentNode.GlobalPosition + _communistPosition;
                ModeManager.CurrentLevel.AddChild(addCommunist);
            };
        }
        else
        {
            //TODO: colocar implementacao de exibir UI indicando que nao tem recurso, voz, etc.
            GD.Print("QUANTIDADE INSUFICIENTE DE RECURSO");
        }
    }
}