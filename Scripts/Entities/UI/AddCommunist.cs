using ClawtopiaCs.Scripts.Systems;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class AddCommunist : Button
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
        Ui = GetNode<UI>("/root/Game/UI");
        MouseEntered += Ui.Enter_ui_mode;
        MouseExited += Ui.Leave_ui_mode;
        Pressed += OnPressed;
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
    }

    public void OnPressed()
    {
        SpawnCommunist();
    }

    private void SpawnCommunist()
    {
        GD.Print("SPAWN");

        var addCommunist = _scene.Instantiate<Ally>();
        var purrlamentNode = ModeManager.CurrentLevel.GetNode<Building>(Constants.COMMUNE_EXTERNAL_NAME);
        LevelManager = GetNode<LevelManager>("/root/Game/LevelManager");

        //INICIA SPAWN DOS GATOS CAMPONESES
        ResourceSpawnTimer = GetTree().CreateTimer(SpawnTimer);

        if (LevelManager.InitialSalmonQuantity >= 100)
        {
            LevelManager.EmitSignal(LevelManager.SignalName.ResourceExpended, Constants.SALMON, 100);
            ResourceSpawnTimer.Timeout += delegate
            {
                addCommunist.GlobalPosition = purrlamentNode.GlobalPosition + _communistPosition;
                ModeManager.CurrentLevel.AddChild(addCommunist);
            };
        }
        else
        {
            GD.Print("QUANTIDADE INSUFICIENTE DE RECURSO");
        }
    }
}