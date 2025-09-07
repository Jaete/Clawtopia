using ClawtopiaCs.Scripts.Entities;
using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Entities.Characters;
using System.Collections.Generic;
using Godot;
using static BuildingData;

public partial class Ally : Unit
{
    [Export(PropertyHint.Enum, Constants.ALLY_CATEGORY_LIST)] public string Category;

    [ExportGroup("UI Settings")]
    [Export] public PackedScene UIMenu;
    // DETECTA SE ALIADO ESTA SELECIONADO
    public bool CurrentlySelected;

    // DETECTA CONSTRUCAO INTERAGIDA, SE HOUVER
    // ABAIXO O MESMO PARA RECURSO
    public Building InteractedBuilding;
    public CollectPoint InteractedCollectPoint;
    public bool Delivering;
    public Vector2 CurrentResourceLastPosition = new();
    public int ResourceCurrentQuantity;
    public bool AllyIsBuilding;
    public bool InteractedWithBuilding;
    public Building ConstructionToBuild;

    public Vector2 LastDirection { get; private set; } = Vector2.Down;

    public override void _Ready()
    {
        Initialize();
    }

    public void Initialize()
    {
        InteractionShape.MouseEntered += OnHover;
        InteractionShape.MouseExited += OnUnhover;
    }

    public void UpdateDirection(Vector2 velocity)
    {
        if (velocity != Vector2.Zero)
        {
            LastDirection = velocity.Normalized();
        }
    }

    public virtual void OnHover()
    {
        Modulation.AssignState(this, InteractionStates.HOVER);
    }
    
    public virtual void OnUnhover()
    {
       Modulation.AssignState(this, InteractionStates.UNHOVER);
    }
}
