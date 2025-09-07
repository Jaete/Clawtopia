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
    public Dictionary<Collectable, int> ResourceCosts = new();
}
