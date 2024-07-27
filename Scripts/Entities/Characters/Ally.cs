using Godot;
using System;

public partial class Ally : CharacterBody2D
{
    // VARIAVEIS DE "CONFIGURACAO" DO PERSONAGEM
    [ExportCategory("Settings")]
    [Export] public Attributes attributes;
    [Export] public StateMachine state_machine;
    [Export] public NavigationAgent2D agent;
    [Export] public AnimatedSprite2D sprite;
    
    // REFERENCIA PARA O NODE DO LEVEL ATUAL
    public Node2D current_level;
    
    // DETECTA SE ALIADO ESTA SELECIONADO
    public bool currently_selected;
    
    // DETECTA CONSTRUCAO INTERAGIDA, SE HOUVER
    // ABAIXO O MESMO PARA RECURSO
    public Building interacted_building;
    public string interacted_resource;
    public bool delivering;
    public Vector2 current_resource_last_position = new();
    public int resource_current_quantity;
    public bool ally_is_building;
    public bool interacted_with_building;
    public Building construction_to_build;
    
    // REFERENCIA PARA O LEVEL MANAGER, QUE TERÁ OS DADOS DE RECURSO DO JOGADOR
    public LevelManager level_manager;
    
    public override void _Ready(){
        CallDeferred("Initialize");
    }

    public void Initialize(){
        level_manager = GetNode<LevelManager>("/root/Game/LevelManager");
        current_level = level_manager.GetNode<Node2D>("Level");
    }
}
