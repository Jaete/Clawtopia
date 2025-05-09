using Godot;
using Godot.Collections;

namespace ClawtopiaCs.Scripts.Systems;

public partial class LevelManager : Node2D
{
    [Signal]
    public delegate void ResourceDeliveredEventHandler(Dictionary<string, int> resources);

    [Signal]
    public delegate void ResourceExpendedEventHandler(Dictionary<string, int> resources);

    public static LevelManager Singleton { get; private set; }

    public Array<Building> CatnipBuildings = new();
    public Array<Building> SalmonBuildings = new();
    public Array<Building> SandBuildings = new();
    
    public Label CatnipLabel;
    public Label SalmonLabel;
    public Label SandLabel;

    public Dictionary<string, int> CurrentResources = new();


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
        ResourceDelivered += When_resource_delivered;
        ResourceExpended += WhenResourceExpended;
        CurrentResources[Constants.SALMON] = InitialSalmonQuantity;
        CurrentResources[Constants.CATNIP] = InitialCatnipQuantity;
        CurrentResources[Constants.SAND] = InitialSandQuantity;
        UpdateLabels();
    }

    private void WhenResourceExpended(Dictionary<string, int> resources)
    {
        foreach (var resource in resources)
        {
            CurrentResources[resource.Key] -= resource.Value;
            UpdateLabels();
        }
    }

    public void When_resource_delivered(Dictionary<string, int> resources)
    {
        foreach (var resource in resources)
        {
            CurrentResources[resource.Key] += resource.Value;
            UpdateLabels();
        }
    }

    private void UpdateLabels()
    {
        SalmonLabel.Text = $"{CurrentResources[Constants.SALMON]}";
        CatnipLabel.Text = $"{CurrentResources[Constants.CATNIP]}";
        SandLabel.Text = $"{CurrentResources[Constants.SAND]}";
    }
}