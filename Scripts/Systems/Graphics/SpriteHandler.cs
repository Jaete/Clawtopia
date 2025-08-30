using Godot;
public partial class SpriteHandler
{
    public static void ChangeSprite(Sprite2D sprite, Texture2D texture)
    {
        sprite.Texture = texture;
    }

    public static void ChangeAnimation(AnimatedSprite2D sprite, string animation, bool flipH)
    {
        sprite.FlipH = flipH;
        sprite.Play(animation);
    }
}
