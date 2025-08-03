using Godot;
using Godot.Collections;
using static BuildingData;


namespace ClawtopiaCs.Scripts.Systems;

public partial class LevelManager : Node2D
{
    [Signal]
    public delegate void ResourceDeliveredEventHandler(Dictionary<ResourceType, int> resources);

    [Signal]
    public delegate void ResourceExpendedEventHandler(Dictionary<ResourceType, int> resources);

    public static LevelManager Singleton { get; private set; }

    public Array<Building> CatnipBuildings = new();
    public Array<Building> SalmonBuildings = new();
    public Array<Building> SandBuildings = new();
    
    public Label CatnipLabel;
    public Label SalmonLabel;
    public Label SandLabel;

    public Dictionary<ResourceType, int> CurrentResources = new();

    [Export] public int InitialCatnipQuantity = 0;
    [Export] public int InitialSalmonQuantity = 0;
    [Export] public int InitialSandQuantity = 0;
    public Building Purrlament;
    public Control ResCount;

    public Array<CollectPoint> CollectPoints = new();

    public UI Ui;

    public override void _Ready()
    {
        Singleton = this;
        Ui = GetNode<UI>("/root/Game/UI");
        ResCount = Ui.GetNode<Control>("ResourcesCount");
        CatnipLabel = ResCount.GetNode<Label>("CatnipLabel");
        SalmonLabel = ResCount.GetNode<Label>("SalmonLabel");
        SandLabel = ResCount.GetNode<Label>("SandLabel");
        ResourceDelivered += WhenResourceDelivered;
        ResourceExpended += WhenResourceExpended;
        CurrentResources[ResourceType.Salmon] = InitialSalmonQuantity;
        CurrentResources[ResourceType.Catnip] = InitialCatnipQuantity;
        CurrentResources[ResourceType.Sand] = InitialSandQuantity;
        UpdateLabels();
    }

    private void WhenResourceExpended(Dictionary<ResourceType, int> resources)
    {
        foreach (var resource in resources)
        {
            CurrentResources[resource.Key] -= resource.Value;
            UpdateLabels();
        }
    }

    public void WhenResourceDelivered(Dictionary<ResourceType, int> resources)
    {
        foreach (var resource in resources)
        {
            CurrentResources[resource.Key] += resource.Value;
            UpdateLabels();
        }
    }

    private void UpdateLabels()
    {
        SalmonLabel.Text = $"{CurrentResources[ResourceType.Salmon]}";
        CatnipLabel.Text = $"{CurrentResources[ResourceType.Catnip]}";
        SandLabel.Text = $"{CurrentResources[ResourceType.Sand]}";
    }
}