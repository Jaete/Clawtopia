using ClawtopiaCs.Scripts.Entities;
using ClawtopiaCs.Scripts.Entities.Building;
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

    public virtual void OnHover()
    {
        Modulation.AssignState(this, InteractionStates.HOVER);
    }
    
    public virtual void OnUnhover()
    {
       Modulation.AssignState(this, InteractionStates.UNHOVER);
    }
}
