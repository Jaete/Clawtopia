using Godot;
using System;

public partial class AddCommunist : Button
{
	public ModeManager mode_manager;
	public LevelManager level_manager;
	public UI ui;
	public SceneTreeTimer resource_spawn_timer;
    
	[Export] public PackedScene communist;
	[Export] public float spawnTimer = 20;
	
	private PackedScene _scene = GD.Load<PackedScene>("res://TSCN/Entities/Characters/Allies/Economic/Economic.tscn");
	private Vector2 _communistPosition = new Vector2(-145.0f, 85.0f);

	public override void _Ready(){
		ui = GetNode<UI>("/root/Game/UI");
		MouseEntered += ui.Enter_ui_mode;
		MouseExited += ui.Leave_ui_mode;
		Pressed += OnPressed;
		mode_manager = GetNode<ModeManager>("/root/Game/ModeManager");
	}

	public void OnPressed() {
		SpawnCommunist();
	}

	private void SpawnCommunist()
	{
		GD.Print("SPAWN");

		var addCommunist = _scene.Instantiate<Ally>();
		var purrlamentNode = mode_manager.current_level.GetNode<Building>("Purrlamento");
		level_manager = GetNode<LevelManager>("/root/Game/LevelManager");

		//INICIA SPAWN DOS GATOS CAMPONESES
		resource_spawn_timer = GetTree().CreateTimer(spawnTimer);

		if (level_manager.salmon_quantity >= 100)
		{
			resource_spawn_timer.Timeout += delegate
			{
				addCommunist.GlobalPosition = purrlamentNode.GlobalPosition + _communistPosition;
				mode_manager.current_level.AddChild(addCommunist);
			};
		}
		else
		{
			GD.Print("QUANTIDADE INSUFICIENTE DE RECURSO");
		}
	}
}
