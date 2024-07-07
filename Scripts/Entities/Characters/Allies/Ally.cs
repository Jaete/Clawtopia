using Godot;
using System;

public partial class Ally : CharacterBody2D
{
    [ExportCategory("Settings")]
    [Export]
    public Attributes attributes;
    [Export]
    public AllyType type;
    [Export]
    public StateMachine state_machine;
    [Export]
    public NavigationAgent2D agent;
    public Node2D current_level;

    public override void _Ready()
    {
        current_level = (Node2D)GetNode("/root/Game/LevelManager/Level");
    }
}
