using ClawtopiaCs.Scripts.Systems;
using Godot;

public partial class Ally : Unit
{
    [Export(PropertyHint.Enum, Constants.ALLY_CATEGORY_LIST)] public string Category;
    
    // REFERENCIA PARA O NODE DO LEVEL ATUAL
    public Node2D CurrentLevel;
    
    // DETECTA SE ALIADO ESTA SELECIONADO
    public bool CurrentlySelected;
    
    // DETECTA CONSTRUCAO INTERAGIDA, SE HOUVER
    // ABAIXO O MESMO PARA RECURSO
    public Building InteractedBuilding;
    public string InteractedResource;
    public bool Delivering;
    public Vector2 CurrentResourceLastPosition = new();
    public int ResourceCurrentQuantity;
    public bool AllyIsBuilding;
    public bool InteractedWithBuilding;
    public Building ConstructionToBuild;
    
    // REFERENCIA PARA O LEVEL MANAGER, QUE TERÁ OS DADOS DE RECURSO DO JOGADOR
    public LevelManager LevelManager;
    
    public override void _Ready(){
        CallDeferred("Initialize");
    }

    public void Initialize(){
        LevelManager = GetNode<LevelManager>("/root/Game/LevelManager");
        CurrentLevel = LevelManager.GetNode<Node2D>("Level");
    }
}
