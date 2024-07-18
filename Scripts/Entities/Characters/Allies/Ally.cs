using Godot;
using System;

public partial class Ally : CharacterBody2D
{
    [ExportCategory("Settings")]
    [Export] public Attributes attributes;
    [Export] public StateMachine state_machine;
    [Export] public NavigationAgent2D agent;
    [Export] public AnimatedSprite2D sprite;
    public Node2D current_level;
    public bool currently_selected;
    public Building interacted_building;
    public Rid level_rid;
    
    public override void _Ready(){
        CallDeferred("Initialize");
    }

    public void Initialize(){
        current_level = GetNode<Node2D>("/root/Game/LevelManager/Level");
        level_rid = current_level.GetNode<NavigationRegion2D>("Navigation").GetRid();
    }
}
