using Godot;
using System.Collections.Generic;

namespace ClawtopiaCs.Scripts.Systems.Audio
{
    [Tool, GlobalClass]
    public partial class UnitAudioController : AudioStreamPlayer2D
    {
        private Dictionary<UnitAudioClip, int> ClipMap = new();

        private AudioStreamPlaybackInteractive _playback;
        private int _currentClipIndex = -1;

        public override void _Ready()
        {
            ClipMap = new Dictionary<UnitAudioClip, int>
            {
                { UnitAudioClip.Move, 0 },
                { UnitAudioClip.Build, 1 },
                { UnitAudioClip.Collect, 2 }
            };

            base._Ready();
        }

        public void SetClip(UnitAudioClip clip)
        {
            if (_playback == null)
            {
                _playback = (AudioStreamPlaybackInteractive)Stream.InstantiatePlayback();
                if (_playback == null) return;
            }

            if (ClipMap.TryGetValue(clip, out int index))
            {
                if (_currentClipIndex == index) return;

                _currentClipIndex = index;
                _playback.SwitchToClip(index);

                if (!Playing)
                    Play();
            }
            else
            {
                GD.PrintErr($"{Name}: o Clipe {clip} nao esta mapeando o ClipMap.");
            }
        }

        public void StopClip()
        {
            Stop();
            _currentClipIndex = -1;
        }
    }
}