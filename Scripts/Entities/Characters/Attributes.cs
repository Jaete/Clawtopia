using Godot;
using Godot.Collections;

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

    public Dictionary<string, int> ResourceCosts = new();

    public void Initialize()
    {
        ResourceCosts[Constants.SALMON] = SalmonCost;
        ResourceCosts[Constants.CATNIP] = CatnipCost;
        ResourceCosts[Constants.SAND] = SandCost;
    }
}
