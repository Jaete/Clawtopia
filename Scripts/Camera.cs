using Godot;

public partial class Camera : Camera2D
{
    public enum CameraMode
    {
        Dragging,
        Sliding
    }

    private float bottomThreshold;
    public CameraMode CameraState = CameraMode.Sliding;
    [Export] public float dragSensitivity = 1.0f;
    public Vector2 initialMousePosition;

    private float leftThreshold;
    [Export] public float MaxZoom = 2.0f;
    [Export] public float MinZoom = 0.5f;
    public Vector2 mousePosition;
    private float rightThreshold;
    [Export] public float Sensitivity = 5.0f;

    [ExportCategory("Camera Properties")] [Export]
    public float Speed = 0.5f;

    private float topThreshold;

    public Rect2 visibleRect;
    [Export] public float ZoomAmmount = 0.3f;

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
        if (CameraState == CameraMode.Sliding) {
            if (OS.IsDebugBuild()) {
                return;
            }

            if (mousePosition.X < leftThreshold + Sensitivity && mousePosition.Y < topThreshold + Sensitivity)
                direction = new Vector2(-1, -1).Normalized();
            else if (mousePosition.X > rightThreshold - Sensitivity && mousePosition.Y < topThreshold + Sensitivity)
                direction = new Vector2(+1, -1).Normalized();
            else if (mousePosition.X < leftThreshold + Sensitivity && mousePosition.Y > bottomThreshold - Sensitivity)
                direction = new Vector2(-1, +1).Normalized();
            else if (mousePosition.X > rightThreshold - Sensitivity && mousePosition.Y > bottomThreshold - Sensitivity)
                direction = new Vector2(+1, +1).Normalized();
            else if (mousePosition.X < leftThreshold + Sensitivity || Input.IsActionPressed("camera_left"))
                direction = new Vector2(-1, 0);
            else if (mousePosition.X > rightThreshold - Sensitivity || Input.IsActionPressed("camera_right"))
                direction = new Vector2(+1, 0);
            else if (mousePosition.Y < topThreshold + Sensitivity || Input.IsActionPressed("camera_up"))
                direction = new Vector2(0, -1);
            else if (mousePosition.Y > bottomThreshold - Sensitivity || Input.IsActionPressed("camera_down"))
                direction = new Vector2(0, +1);

            Position += direction * Speed / Zoom.X;
        }


        if (CameraState == CameraMode.Dragging) {
            Vector2 deltaMousePosition = mousePosition - initialMousePosition;
            Position -= deltaMousePosition * dragSensitivity / Zoom.X;
            initialMousePosition = mousePosition;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed("scroll_up")) {
            Zoom += new Vector2(ZoomAmmount, ZoomAmmount);
        }

        else if (Input.IsActionPressed("scroll_down")) {
            Zoom -= new Vector2(ZoomAmmount, ZoomAmmount);
        }

        Zoom = new Vector2(Mathf.Clamp(Zoom.X, MinZoom, MaxZoom), Mathf.Clamp(Zoom.Y, MinZoom, MaxZoom));

        if (@event is InputEventMouseButton mouseButtonEvent) {
            if (mouseButtonEvent.ButtonIndex == MouseButton.Middle && mouseButtonEvent.Pressed) {
                CameraState = CameraMode.Dragging;
                initialMousePosition = mousePosition;
            }
            else if (mouseButtonEvent.ButtonIndex == MouseButton.Middle && !mouseButtonEvent.Pressed) {
                CameraState = CameraMode.Sliding;
            }
        }
    }
}