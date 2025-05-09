using ClawtopiaCs.Scripts.Systems;
using Godot;
using System;

public partial class CollectPoint : StaticBody2D
{
    public Color HoverColor = new Color(1.3f, 1.3f, 1.3f);
    public Color NormalColor = new Color(1f, 1f, 1f);
    public int SelfIndex;

    [Export] Area2D Interaction;

    public override void _Ready()
    {
        CallDeferred(MethodName.Initialize);
    }

    public void Initialize()
    {
        InputPickable = true;
        LevelManager.Singleton.CollectPoints.Add(this);
        SelfIndex = LevelManager.Singleton.CollectPoints.Count;
        Interaction.MouseEntered += OnHover;
        Interaction.MouseExited += OnUnhover;
    }

    public void OnHover()
    {
        Modulate = HoverColor;
    }

    public void OnUnhover()
    {
        Modulate = NormalColor;
    }
}
