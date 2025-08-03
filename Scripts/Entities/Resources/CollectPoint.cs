using ClawtopiaCs.Scripts.Entities;
using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Systems;
using Godot;
using static BuildingData;

public partial class CollectPoint : StaticBody2D
{
    [Signal] public delegate void ResourceCollectedEventHandler(int quantity);


    private int _resourceQuantity = 0;
    public int SelfIndex;

    [Export(PropertyHint.Enum, Constants.RESOURCE_LIST)] public ResourceType ResourceType;

    [Export] public Area2D Interaction;
    [Export] public ProgressStructure Structure;
    [Export] public int MaxResourceQuantity;

    public int ResourceQuantity { 
        get => _resourceQuantity;
        set {
            _resourceQuantity = value;
            if (_resourceQuantity - value < 0)
            {
                _resourceQuantity = 0;
            }
            ChangeSpriteOnBreakpoint();
        }
    }

    public override void _Ready()
    {
        CallDeferred(MethodName.Initialize);
    }

    public void Initialize()
    {
        InputPickable = true;
        LevelManager.Singleton.CollectPoints.Add(this);
        SelfIndex = LevelManager.Singleton.CollectPoints.Count;
        Interaction.MouseEntered += OnHover;
        Interaction.MouseExited += OnUnhover;
        ResourceCollected += OnResourceCollected;
        ResourceQuantity = MaxResourceQuantity;
    }

    private void OnResourceCollected(int quantity)
    {
        ResourceQuantity -= quantity;
    }

    public virtual void OnHover()
    {
        Modulation.AssignState(this, InteractionStates.HOVER);
    }

    public virtual void OnUnhover()
    {
       Modulation.AssignState(this, InteractionStates.UNHOVER);
    }
    public virtual void ChangeSpriteOnBreakpoint() { }
}

