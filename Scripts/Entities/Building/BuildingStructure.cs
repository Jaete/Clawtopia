using Godot;
using Godot.Collections;

namespace ClawtopiaCs.Scripts.Entities.Building
{
    public partial class BuildingStructure : Resource
    {
        [Export] public Texture2D PreviewTexture;
        [Export] public Texture2D PlacedTexture;
        [Export] public Vector2 PlacedOffset;
        [Export] public ConcavePolygonShape2D Collision;
        [Export] public ConcavePolygonShape2D Interaction;
        [Export] public Texture2D RotatedPreviewTexture;
        [Export] public Texture2D RotatedPlacedTexture;
        [Export] public Vector2 RotatedPlacedOffset;
        [Export] public ConcavePolygonShape2D RotatedCollision;
        [Export] public ConcavePolygonShape2D RotatedInteraction;
    }
}
