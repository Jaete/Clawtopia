using Godot;
using System;

[GlobalClass]
public partial class ResourceLabel : Label
{
    [Export]
    public Collectable Collectable { get; set; }
}
