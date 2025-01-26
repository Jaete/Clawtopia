using Godot;

namespace ClawtopiaCs.Scripts.Entities.UI
{
    public partial class UIButton : Button
    {
        public AudioStreamPlayer SoundPlayer = new();
        [Export] public AudioStream ClickSound = GD.Load<AudioStream>("res://Assets/Audio/UI/ui-click.ogg");

        public override void _Ready()
        {
            GD.Print($"PLAYER ID: {SoundPlayer.GetInstanceId()}");
            SoundPlayer.Stream = ClickSound;
            AddChild(SoundPlayer);
        }

        public virtual void OnPressed()
        {
            SoundPlayer.Play();
        }
    }
}
