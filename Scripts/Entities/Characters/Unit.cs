
using ClawtopiaCs.Scripts.Entities;
using ClawtopiaCs.Scripts.Entities.Building;
using Godot;

public partial class Unit : CharacterBody2D
{
    [ExportCategory("Settings")]
    [Export] public Attributes Attributes;
    [Export] public StateMachine StateMachine;
    [Export] public NavigationAgent2D Navigation;
    [Export] public AnimatedSprite2D Sprite;
    [Export] public Area2D InteractionShape;
    [Export] public AudioStreamPlayer2D AudioPlayer;
}
