using Godot;
using System;
public partial class CustomCursor : Node
{
    public static CustomCursor Instance;

    public static Texture2D defaultCursor;
    private Texture2D foiceCursor;
    private Texture2D paCursor;
    private Texture2D varaCursor;

    public override void _Ready()
    {
        if (Instance == null)
            Instance = this;
        else
            QueueFree();
        defaultCursor = GD.Load<Texture2D>("res://Assets/UI/New UI/cursor32x32.png");
        foiceCursor = GD.Load<Texture2D>("res://Assets/UI/New UI/scickle-mouse.png");
        paCursor = GD.Load<Texture2D>("res://Assets/UI/New UI/shovel-mouse.png");
        varaCursor = GD.Load<Texture2D>("res://Assets/UI/New UI/fishing-rod-mouse.png");
        ResetCursor();
    }

    public void SetCursor(Texture2D cursor)
    {
        Input.SetCustomMouseCursor(cursor);
    }

    public void ResetCursor()
    {
        SetCursor(defaultCursor);
    }

    public enum CursorType
    {
        Default,
        Foice,
        Pa,
        Vara
    }
}