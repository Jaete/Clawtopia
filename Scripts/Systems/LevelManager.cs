using System.Linq;
using Godot;
using Godot.Collections;
using static BuildingData;


namespace ClawtopiaCs.Scripts.Systems;

public partial class LevelManager : Node2D
{
    [Signal]
    public delegate void ResourceDeliveredEventHandler(Dictionary<Collectable, int> resources);

    [Signal]
    public delegate void ResourceExpendedEventHandler(Dictionary<Collectable, int> resources);

    public static LevelManager Singleton { get; private set; }

    public Array<Building> CollectorBuildings = new();

    public Array<ResourceLabel> ResourceLabels { get; set; }

    [Export] public Dictionary<Collectable, int> InitialResources { get; set; }

    public Dictionary<Collectable, int> CurrentResources { get; set; } = new();
    public Building Purrlament;
    public Control ResCount;

    public Array<CollectPoint> CollectPoints = new();

    public UI Ui;

    public override void _Ready()
    {
        Singleton = this;
        Ui = GetNode<UI>("/root/Game/UI");
        ResCount = Ui.GetNode<Control>("ResourcesCount");
        ResourceLabels = new Array<ResourceLabel>();
        ResCount.GetNode("Labels").GetChildren().ToList().ForEach(c => {
            if (c is ResourceLabel label)
            {
                ResourceLabels.Add(label);
            }
        });
        
        ResourceDelivered += WhenResourceDelivered;
        ResourceExpended += WhenResourceExpended;
        foreach (var item in InitialResources)
        {
            CurrentResources.Add(item.Key, item.Value);
        }
        UpdateLabels(CurrentResources);
    }

    private void WhenResourceExpended(Dictionary<Collectable, int> resources)
    {
        foreach (var resource in resources)
        {
            CurrentResources[resource.Key] -= resource.Value;
            UpdateLabels(CurrentResources);
        }
    }

    public void WhenResourceDelivered(Dictionary<Collectable, int> resources)
    {
        foreach (var resource in resources)
        {
            CurrentResources[resource.Key] += resource.Value;
            UpdateLabels(CurrentResources);
        }
    }

    private void UpdateLabels(Dictionary<Collectable, int> resources)
    {
        foreach (var item in resources)
        {
            ResourceLabels
                .FirstOrDefault(lb => lb.Collectable == item.Key)
                .Text = item.Value.ToString();
        }
    }
}