using Godot;
using System.Collections.Generic;
using ClawtopiaCs.Scripts.Entities.Characters;

public partial class AnimController : Node 
{
    public Dictionary<CharacterAnim, string> animMap { get; private set; } = new();

    public void SetAnimMap(Dictionary<CharacterAnim, string> map)
    {
        animMap = map;
    }
}