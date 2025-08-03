using ClawtopiaCs.Scripts.Systems;
using Godot;
using Godot.Collections;
using ClawtopiaCs.Scripts.Systems.GameModes;

public partial class CollectPoint : StaticBody2D
{
    [Signal] public delegate void ResourceCollectedEventHandler(int quantity);


    private int _resourceQuantity = 0;

    public Color HoverColor = new Color(1.3f, 1.3f, 1.3f);
    public Color NormalColor = new Color(1f, 1f, 1f);
    public int SelfIndex;

    [Export(PropertyHint.Enum, Constants.RESOURCE_LIST)] public string ResourceType;

    [Export] public Area2D Interaction;
    [Export] public ProgressStructure Structure;
    [Export] public int MaxResourceQuantity;

    public int ResourceQuantity
    {
        get => _resourceQuantity;
        set
        {
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
        Modulate = HoverColor;

        if (SimulationMode.Singleton.SelectedAllies.Count > 0)
        {
            switch (ResourceType.ToLower())
            {
                case "salmon":
                    CustomCursor.Instance.SetCursor(CustomCursor.CursorType.Vara);
                    break;
                case "sand":
                    CustomCursor.Instance.SetCursor(CustomCursor.CursorType.Pa);
                    break;
                case "catnip":
                    CustomCursor.Instance.SetCursor(CustomCursor.CursorType.Foice);
                    break;
                default:
                    CustomCursor.Instance.SetCursor(CustomCursor.CursorType.Default);
                    break;
            }
        }
    }
    
    public virtual void OnUnhover()
    {
        Modulate = NormalColor;

        CustomCursor.Instance.SetCursor(CustomCursor.CursorType.Default);
    }
    public virtual void ChangeSpriteOnBreakpoint() { }
}

