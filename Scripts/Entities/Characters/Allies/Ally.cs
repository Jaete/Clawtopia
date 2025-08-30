using ClawtopiaCs.Scripts.Entities;
using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Entities.Characters;
using System.Collections.Generic;
using Godot;

public partial class Ally : Unit
{
    [Export(PropertyHint.Enum, Constants.ALLY_CATEGORY_LIST)] public string Category;
    // DETECTA SE ALIADO ESTA SELECIONADO
    public bool CurrentlySelected;

    // DETECTA CONSTRUCAO INTERAGIDA, SE HOUVER
    // ABAIXO O MESMO PARA RECURSO
    public Building InteractedBuilding;
    public CollectPoint InteractedResource;
    public bool Delivering;
    public Vector2 CurrentResourceLastPosition = new();
    public int ResourceCurrentQuantity;
    public bool AllyIsBuilding;
    public bool InteractedWithBuilding;
    public Building ConstructionToBuild;

    public Vector2 LastDirection { get; private set; } = Vector2.Down;

    public override void _EnterTree()
    {
        AnimController.SetAnimMap(new Dictionary<CharacterAnim, string>
        {
            { CharacterAnim.BuildingLeft, "building_left" },
            { CharacterAnim.BuildingRight, "building_right" },

            { CharacterAnim.CatnipLeft, "catnip_left" },
            { CharacterAnim.CatnipRight, "catnip_right" },
            { CharacterAnim.FishingLeft, "fishing_left" },
            { CharacterAnim.FishingRight, "fishing_right" },
            { CharacterAnim.SandLeft, "sand_left" },
            { CharacterAnim.SandRight, "sand_right" },

            { CharacterAnim.DeadLeft, "dead_left" },
            { CharacterAnim.DeadRight, "dead_right" },

            { CharacterAnim.IdleLeft, "idle_left" },
            { CharacterAnim.IdleRight, "idle_right" },

            { CharacterAnim.MoveDown, "move_down" },
            { CharacterAnim.MoveDownLeft, "move_down_left" },
            { CharacterAnim.MoveDownRight, "move_down_right" },
            { CharacterAnim.MoveLeft, "move_left" },
            { CharacterAnim.MoveRight, "move_right" },
            { CharacterAnim.MoveUp, "move_up" },
            { CharacterAnim.MoveUpLeft, "move_up_left" },
            { CharacterAnim.MoveUpRight, "move_up_right" }
        });
    }
    public override void _Ready()
    {
        Initialize();
    }

    public void Initialize()
    {
        Attributes.Initialize();
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
