using Godot;
using System;
public partial class CustomCursor : Node
{
    public static CustomCursor Instance;

    private Texture2D defaultCursor;
    private Texture2D foiceCursor;
    private Texture2D paCursor;
    private Texture2D varaCursor;

    public override void _Ready()
    {
        if (Instance == null)
            Instance = this;
        else
            QueueFree();
        defaultCursor = null;
        foiceCursor = GD.Load<Texture2D>("res://Assets/UI/New UI/scickle-mouse.png");
        paCursor = GD.Load<Texture2D>("res://Assets/UI/New UI/shovel-mouse.png");
        varaCursor = GD.Load<Texture2D>("res://Assets/UI/New UI/fishing-rod-mouse.png");
    }

    public void SetCursor(CursorType type)
    {
        Texture2D cursor = type switch
        {
            CursorType.Default => defaultCursor,
            CursorType.Foice => foiceCursor,
            CursorType.Pa => paCursor,
            CursorType.Vara => varaCursor,
        };
        Input.SetCustomMouseCursor(cursor);
    }

    public enum CursorType
    {
        Default,
        Foice,
        Pa,
        Vara
    }
}