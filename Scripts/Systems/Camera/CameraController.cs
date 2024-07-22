using Godot;
using System;


public partial class CameraController : Camera2D
{
    [ExportCategory("Camera Properties")]
    [Export] public float Speed = 0.1f;
    [Export] public int Sensitivity = 20;
    [Export] public float MaxZoom = 2.0f;
    [Export] public float MinZoom = 0.1f;

    public override void _Ready()
    {
        // Smooth camera movement (less precise but visually smoother)
        base.PositionSmoothingEnabled = true;
        base.PositionSmoothingSpeed = 3.5f; 
    }

    public override void _Process(double delta)
    {
        var mousePosition = GetViewport().GetMousePosition();
        var viewportSize = GetViewport().GetVisibleRect().Size;

        var leftThreshold = Sensitivity;
        var rightThreshold = viewportSize.X - Sensitivity;
        var topThreshold = Sensitivity;
        var bottomThreshold = viewportSize.Y - Sensitivity;

        var direction = Vector2.Zero;
        if (mousePosition.X < leftThreshold)
            direction = Vector2.Left;
        else if (mousePosition.X > rightThreshold)
            direction = Vector2.Right;
        else if (mousePosition.Y < topThreshold)
            direction = Vector2.Up;
        else if (mousePosition.Y > bottomThreshold)
            direction = Vector2.Down;

        Position += direction * Speed; // Equivalent to 'position +='
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed("scroll_up"))
        {
            Zoom = new Vector2(Mathf.Min(Zoom.X + 0.1f, MaxZoom), Mathf.Min(Zoom.X + 0.1f, MaxZoom));
            GD.Print(Zoom.X);
        }

        if (Input.IsActionPressed("scroll_down"))
        {
            Zoom = new Vector2(Mathf.Max(Zoom.X - 0.1f, MinZoom), Mathf.Max(Zoom.X - 0.1f, MinZoom));
            GD.Print(Zoom.Y);
        }
    }
}
