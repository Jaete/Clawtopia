using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[ExportCategory("Settings")]
	[Export] public Attributes attributes;
	[Export] public StateMachine state_machine;
	[Export] public NavigationAgent2D agent;
	[Export] public AnimatedSprite2D sprite;
	[Export] public string category;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
