using Godot;
using Godot.Collections;

namespace ClawtopiaCs.Scripts.Entities.Building
{
    public partial class BuildingStructure : Resource
    {
        [Export] public Texture2D Texture;
        [Export] public ConcavePolygonShape2D Collision;
        [Export] public ConcavePolygonShape2D Interaction;
    }
}
