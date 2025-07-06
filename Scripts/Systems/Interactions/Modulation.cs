using ClawtopiaCs.Scripts.Systems.Interactions;
using Godot;

namespace ClawtopiaCs.Scripts.Entities.Building
{
    public class Modulation
    {
        public static void AssignState(Node2D node, InteractionStates state)
        {
            node.Modulate = StateInteractions.Colors[state];
        }
    }
}
