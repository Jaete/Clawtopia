using Godot;


public partial class CameraController : Camera2D
{
    [ExportCategory("Camera Properties")]
    [Export] public float Speed = 0.1f;
    [Export] public int Sensitivity = 20;
    [Export] public float MaxZoom = 2.0f;
    [Export] public float MinZoom = 0.1f;
    [Export] public float SmoothSpeed = 0.0f;

    private bool _isGameFocused;

    private Vector2 _mousePosition;
    private Vector2 _viewportSize;

    private int _leftThreshold;
    private int _rightThreshold;
    private int _topThreshold;
    private int _bottomThreshold;
    private Vector2 _currentDirection;

    public override void _Ready()
    {
        // Smooth camera movement (less precise but visually smoother)
        PositionSmoothingEnabled = true;
        PositionSmoothingSpeed = SmoothSpeed;
        _viewportSize = GetViewport().GetVisibleRect().Size;
        GetViewport().SizeChanged += When_viewport_changed;
        GetWindow().FocusExited += When_game_not_focused;
        GetWindow().FocusEntered += When_game_has_focus;
    }

    public override void _Process(double delta)
    {

        if(!_isGameFocused){
            return;
        }
        _mousePosition = GetViewport().GetMousePosition();

        _leftThreshold  = Sensitivity;
        _rightThreshold  = (int) _viewportSize.X - Sensitivity;
        _topThreshold  = Sensitivity;
        _bottomThreshold  = (int) _viewportSize.Y - Sensitivity;

        _currentDirection = Vector2.Zero;

        if (_mousePosition.X < _leftThreshold && _mousePosition.Y < _topThreshold){
            _currentDirection = new Vector2(Vector2.Left.X, Vector2.Up.Y);
        }
            
        else if (_mousePosition.X > _rightThreshold && _mousePosition.Y < _topThreshold){
            _currentDirection = new Vector2(Vector2.Right.X, Vector2.Up.Y);
        }
            
        else if (_mousePosition.X < _leftThreshold && _mousePosition.Y > _bottomThreshold){
            _currentDirection = new Vector2(Vector2.Left.X, Vector2.Down.Y);
        }
            
        else if (_mousePosition.X > _rightThreshold && _mousePosition.Y > _bottomThreshold){
            _currentDirection = new Vector2(Vector2.Right.X, Vector2.Down.Y);
        }
        else if (_mousePosition.X < _leftThreshold){
            _currentDirection = Vector2.Left;
        }
            
        else if (_mousePosition.X > _rightThreshold){
            _currentDirection = Vector2.Right;
        }
            
        else if (_mousePosition.Y < _topThreshold){
            _currentDirection = Vector2.Up;
        }
            
        else if (_mousePosition.Y > _bottomThreshold){
            _currentDirection = Vector2.Down;
        }
            

        Position += _currentDirection * Speed; // Equivalent to 'position +='
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
        _viewportSize = GetViewport().GetVisibleRect().Size;
    }

    public void When_game_not_focused(){
        _isGameFocused = false;
    }

    public void When_game_has_focus(){
        _isGameFocused = true;
    }
}
