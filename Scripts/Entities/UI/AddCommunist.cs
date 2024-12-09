using Godot;
using System;

public partial class AddCommunist : Button
{
	public ModeManager ModeManager;
	public ClawtopiaCs.Scripts.Systems.LevelManager LevelManager;
	public UI Ui;
	public SceneTreeTimer ResourceSpawnTimer;
    
	[Export] public PackedScene Communist;
	[Export] public float SpawnTimer = OS.IsDebugBuild() ? 1 : 20;
	
	private PackedScene _scene = GD.Load<PackedScene>("res://TSCN/Entities/Characters/Allies/Economic/Economic.tscn");
	private Vector2 _communistPosition = new Vector2(-145.0f, 85.0f);

	public override void _Ready(){
		Ui = GetNode<UI>("/root/Game/UI");
		MouseEntered += Ui.Enter_ui_mode;
		MouseExited += Ui.Leave_ui_mode;
		Pressed += OnPressed;
		ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
	}

	public void OnPressed() {
		SpawnCommunist();
	}

	private void SpawnCommunist()
	{
		GD.Print("SPAWN");

		var addCommunist = _scene.Instantiate<Ally>();
		var purrlamentNode = ModeManager.CurrentLevel.GetNode<Building>(Constants.COMMUNE_EXTERNAL_NAME);
		LevelManager = GetNode<ClawtopiaCs.Scripts.Systems.LevelManager>("/root/Game/LevelManager");

		//INICIA SPAWN DOS GATOS CAMPONESES
		ResourceSpawnTimer = GetTree().CreateTimer(SpawnTimer);

		if (LevelManager.SalmonQuantity >= 100){
			LevelManager.EmitSignal(ClawtopiaCs.Scripts.Systems.LevelManager.SignalName.ResourceExpended, Constants.SALMON, 100);
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
