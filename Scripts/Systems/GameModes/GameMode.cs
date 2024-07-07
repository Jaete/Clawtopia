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
        director = (Director)GetNode("/root/Game/Director");
        mode_manager = (ModeManager)GetParent();
        MousePressed += detect_pressed;
        MouseReleased += detect_released;
    }

    public virtual void enter() { }
    public virtual void exit() { }
    public virtual void update() { }

    public virtual void detect_pressed(Vector2 coords) { }
    public virtual void detect_released(Vector2 coords) { }
}
