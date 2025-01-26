using Godot;
using Godot.Collections;

namespace ClawtopiaCs.Scripts.Systems;

public partial class LevelManager : Node2D
{
    [Signal]
    public delegate void ResourceDeliveredEventHandler(Dictionary<string, int> resources);

    [Signal]
    public delegate void ResourceExpendedEventHandler(Dictionary<string, int> resources);

    public Array<Building> CatnipBuildings = new();
    public Label CatnipLabel;

    public Dictionary<string, int> CurrentResources = new();

    // VARIAVEIS DE QUANTIDADE DE RECURSO ATUAL
    [Export] public int InitialCatnipQuantity = 0;
    [Export] public int InitialSalmonQuantity = 0;
    [Export] public int InitialSandQuantity = 0;
    public Building Purrlament;
    public Control ResCount;

    //NODES DE ESTRUTURA DE RECURSO DE SALMAO, CATNIP E AREIA PELO MAPA
    public Array<Building> SalmonBuildings = new();
    public Label SalmonLabel;
    public Array<Building> SandBuildings = new();

    public Label SandLabel;
    // AS CONSTRUCOES VAO INSERIR A SI MESMAS AQUI SEMPRE QUE INSTANCIADAS

    public UI Ui;

    public override void _Ready()
    {
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