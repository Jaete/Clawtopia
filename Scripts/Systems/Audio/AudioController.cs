using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClawtopiaCs.Scripts.Systems.Audio
{
    [Tool]
    public abstract partial class AudioController<T> : Node where T : struct, Enum
    {
        public AudioStreamPlayer2D audioPlayer;

        public Dictionary<T, int> ClipMap { get; set; } = new();

        private AudioStreamPlaybackInteractive _playback;
        private int _currentClipIndex = -1;

        public override void _Ready()
        {
            audioPlayer = GetChildren().OfType<AudioStreamPlayer2D>().FirstOrDefault();

            if (audioPlayer == null)
            {
                GD.PrintErr($"{Name}: audioPlayer nao encontrado.");
                return;
            }

            _playback = audioPlayer.GetStreamPlayback() as AudioStreamPlaybackInteractive;
        }  
    }
}