using Godot;
using System;
using System.Collections.Generic;

public partial class AudioController<T> : Node where T : Enum
{
    [Export] public NodePath PlayerPath;
    public AudioStreamPlayer2D Player { get; private set; }
    private AudioStreamPlaybackInteractive _playback;

    public Dictionary<T, int> ClipMap = new();
    private int _currentClipIndex = -1;

    public override void _Ready()
    {
        Player = GetNode<AudioStreamPlayer2D>(PlayerPath);
        _playback = Player.GetStreamPlayback() as AudioStreamPlaybackInteractive;

        if (_playback == null)
            GD.PrintErr("AudioPlayer não é interactive");
    }

    public void SetClip(T clip)
    {
        if (_playback == null) 
            return;

        int index = ClipMap[clip];

        if (_currentClipIndex == index) 
            return;

        _currentClipIndex = index;
        _playback.SwitchToClip(index);

        if (!Player.Playing)
            Player.Play();
    }

    public T GetCurrentClip()
    {
        foreach (var kv in ClipMap)
            if(kv.Value == _currentClipIndex)
                return kv.Key;

        return default;
    }
}