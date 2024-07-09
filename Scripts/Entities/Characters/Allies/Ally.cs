using Godot;
using System;

public partial class Ally : CharacterBody2D
{
    [ExportCategory("Settings")]
    [Export]
    public Attributes attributes;
    [Export]
    public StateMachine state_machine;
    [Export]
    public NavigationAgent2D agent;
    public Node2D current_level;

    public override void _Ready()
    {
        current_level = GetNode<Node2D>("/root/Game/LevelManager/Level");
    }
}
