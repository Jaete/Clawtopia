using Godot;
using System;


public partial class CameraController : Camera2D
{
    [ExportCategory("Camera Properties")]
    [Export] public float Speed = 0.1f;
    [Export] public int Sensitivity = 20;
    [Export] public float MaxZoom = 2.0f;
    [Export] public float MinZoom = 0.1f;
    [Export] public float SmoothSpeed = 0.0f;

    private bool is_game_focused;

    private Vector2 mousePosition;
    private Vector2 viewportSize;

    private int leftThreshold;
    private int rightThreshold;
    private int topThreshold;
    private int bottomThreshold;
    private Vector2 current_direction;

    public override void _Ready()
    {
        // Smooth camera movement (less precise but visually smoother)
        PositionSmoothingEnabled = true;
        PositionSmoothingSpeed = SmoothSpeed;
        viewportSize = GetViewport().GetVisibleRect().Size;
        GetViewport().SizeChanged += When_viewport_changed;
        GetWindow().FocusExited += When_game_not_focused;
        GetWindow().FocusEntered += When_game_has_focus;
    }

    public override void _Process(double delta)
    {

        if(!is_game_focused){
            return;
        }
        mousePosition = GetViewport().GetMousePosition();

        leftThreshold  = Sensitivity;
        rightThreshold  = (int) viewportSize.X - Sensitivity;
        topThreshold  = Sensitivity;
        bottomThreshold  = (int) viewportSize.Y - Sensitivity;

        current_direction = Vector2.Zero;

        if (mousePosition.X < leftThreshold && mousePosition.Y < topThreshold){
            current_direction = new Vector2(Vector2.Left.X, Vector2.Up.Y);
        }
            
        else if (mousePosition.X > rightThreshold && mousePosition.Y < topThreshold){
            current_direction = new Vector2(Vector2.Right.X, Vector2.Up.Y);
        }
            
        else if (mousePosition.X < leftThreshold && mousePosition.Y > bottomThreshold){
            current_direction = new Vector2(Vector2.Left.X, Vector2.Down.Y);
        }
            
        else if (mousePosition.X > rightThreshold && mousePosition.Y > bottomThreshold){
            current_direction = new Vector2(Vector2.Right.X, Vector2.Down.Y);
        }
        else if (mousePosition.X < leftThreshold){
            current_direction = Vector2.Left;
        }
            
        else if (mousePosition.X > rightThreshold){
            current_direction = Vector2.Right;
        }
            
        else if (mousePosition.Y < topThreshold){
            current_direction = Vector2.Up;
        }
            
        else if (mousePosition.Y > bottomThreshold){
            current_direction = Vector2.Down;
        }
            

        Position += current_direction * Speed; // Equivalent to 'position +='
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed("ScrollUp"))
        {
            Zoom = new Vector2(Mathf.Min(Zoom.X + 0.1f, MaxZoom), Mathf.Min(Zoom.X + 0.1f, MaxZoom));
        }

        if (Input.IsActionPressed("ScrollDown"))
        {
            Zoom = new Vector2(Mathf.Max(Zoom.X - 0.1f, MinZoom), Mathf.Max(Zoom.X - 0.1f, MinZoom));
        }
    }

    public void When_viewport_changed(){
        viewportSize = GetViewport().GetVisibleRect().Size;
    }

    public void When_game_not_focused(){
        is_game_focused = false;
    }

    public void When_game_has_focus(){
        is_game_focused = true;
    }
}
