using Godot;
public partial class SpriteHandler
{
    public static void ChangeSprite(Sprite2D sprite, Texture2D texture, Vector2? offset = null)
    {
        sprite.Texture = texture;
        if (offset is not null)
        {
            sprite.Offset += offset.Value;
        }
    }

    public static void ChangeAnimation(AnimatedSprite2D sprite, string animation)
    {
        sprite.Play(animation);
    }
}
