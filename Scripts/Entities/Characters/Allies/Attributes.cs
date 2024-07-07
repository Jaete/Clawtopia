using Godot;
using System;

public partial class Attributes : Resource
{
    [Export]
    public int hp;
    [Export]
    public int damage;
    [Export]
    public float move_speed;
    [Export]
    public float attack_speed;
    [Export]
    public int vision_range;
    [Export]
    public int attack_range;
}
