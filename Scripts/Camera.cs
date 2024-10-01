using Godot;
using System;

public partial class Camera : Camera2D
{
    [ExportCategory("Camera Properties")] [Export]
    public float Speed = 0.5f;

    [Export] public float Sensitivity = 25.0f;
    [Export] public float dragSensitivity = 1.0f;
    [Export] public float ZoomAmmount = 0.3f;
    [Export] public float MaxZoom = 2.0f;
    [Export] public float MinZoom = 0.5f;

    public Rect2 visibleRect;
    public Vector2 mousePosition;
    public Vector2 initialMousePosition;
    public bool dragging;

    private float leftThreshold;
    private float rightThreshold;
    private float topThreshold;
    private float bottomThreshold;

    public override void _Ready()
    {
        visibleRect = GetViewport().GetVisibleRect();

        leftThreshold = visibleRect.Position.X;
        rightThreshold = visibleRect.Size.X;
        topThreshold = visibleRect.Position.Y;
        bottomThreshold = visibleRect.Size.Y;
    }

    public override void _Process(double delta)
    {
        mousePosition = GetViewport().GetMousePosition();

        Vector2 direction = new Vector2(0, 0);
        if (!dragging)
        {
            if (mousePosition.X < leftThreshold + Sensitivity && mousePosition.Y < topThreshold + Sensitivity)
                direction = new Vector2(-1, -1).Normalized();
            else if (mousePosition.X > rightThreshold - Sensitivity && mousePosition.Y < topThreshold + Sensitivity)
                direction = new Vector2(+1, -1).Normalized();
            else if (mousePosition.X < leftThreshold + Sensitivity && mousePosition.Y > bottomThreshold - Sensitivity)
                direction = new Vector2(-1, +1).Normalized();
            else if (mousePosition.X > rightThreshold - Sensitivity && mousePosition.Y > bottomThreshold - Sensitivity)
                direction = new Vector2(+1, +1).Normalized();
            else if (mousePosition.X < leftThreshold + Sensitivity)
                direction = new Vector2(-1, 0);
            else if (mousePosition.X > rightThreshold - Sensitivity)
                direction = new Vector2(+1, 0);
            else if (mousePosition.Y < topThreshold + Sensitivity)
                direction = new Vector2(0, -1);
            else if (mousePosition.Y > bottomThreshold - Sensitivity)
                direction = new Vector2(0, +1);

            Position += direction * Speed / Zoom.X;
        }

        if (dragging)
        {
            Vector2 mouseDelta = mousePosition - initialMousePosition;
            Position -= mouseDelta * dragSensitivity / Zoom.X;
            initialMousePosition = mousePosition;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed("scroll_up"))
        {
            Zoom += new Vector2(ZoomAmmount, ZoomAmmount);
        }
        else if (Input.IsActionPressed("scroll_down"))
        {
            Zoom -= new Vector2(ZoomAmmount, ZoomAmmount);
        }

        Zoom = new Vector2(Mathf.Clamp(Zoom.X, MinZoom, MaxZoom), Mathf.Clamp(Zoom.Y, MinZoom, MaxZoom));

        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.ButtonIndex == MouseButton.Middle && mouseButtonEvent.Pressed)
            {
                initialMousePosition = GetViewport().GetMousePosition();
                dragging = true;
            }
            else if (mouseButtonEvent.ButtonIndex == MouseButton.Middle && !mouseButtonEvent.Pressed)
            {
                dragging = false;
            }
        }
    }
}