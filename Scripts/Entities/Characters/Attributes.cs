using Godot;
using Godot.Collections;
using static BuildingData;

public partial class Attributes : Resource
{
    [Export]
    public int Hp;
    [Export]
    public int Damage;
    [Export]
    public float MoveSpeed;
    [Export]
    public float AttackSpeed;
    [Export]
    public int VisionRange;
    [Export]
    public int AttackRange;
    [Export]
    public int SalmonCost;
    [Export]
    public int CatnipCost;
    [Export]
    public int SandCost;

    public Dictionary<ResourceType, int> ResourceCosts = new();

    public void Initialize()
    {
        ResourceCosts[ResourceType.Salmon] = SalmonCost;
        ResourceCosts[ResourceType.Catnip] = CatnipCost;
        ResourceCosts[ResourceType.Sand] = SandCost;
    }
}
