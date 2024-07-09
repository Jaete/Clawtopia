using Godot;
using Godot.Collections;
using System;
using System.Threading;

public partial class GameMode : Node2D {
    [Signal]
    public delegate void ModeTransitionEventHandler(String mode, String building, String type);
    [Signal]
    public delegate void MouseClickedEventHandler();
    [Signal]
    public delegate void MousePressedEventHandler(Vector2 coords);
    [Signal]
    public delegate void MouseReleasedEventHandler(Vector2 coords);

    public Director director;
    public Node2D current_level;

    public ModeManager mode_manager;

    public override void _Ready(){
        director = GetNode<Director>("/root/Game/Director");
        mode_manager = GetParent<ModeManager>();
        MousePressed += Detect_pressed;
        MouseReleased += Detect_released;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }

    public virtual void Detect_pressed(Vector2 coords) { }
    public virtual void Detect_released(Vector2 coords) { }
}
