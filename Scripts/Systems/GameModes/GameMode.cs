using Godot;
using Godot.Collections;
using System;
using System.Threading;

public partial class GameMode : Node2D {
    [Signal]
    public delegate void ModeTransitionEventHandler(String mode, String building, String type);
    

    public Director director;
    public Node2D current_level;

    public ModeManager mode_manager;
    
    public Controller controller;

    public override void _Ready(){
        director = GetNode<Director>("/root/Game/Director");
        mode_manager = GetParent<ModeManager>();
        controller = GetNode<Controller>("/root/Game/Controller");
        controller.MousePressed += When_detect_pressed;
        controller.MouseReleased += When_detect_released;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }

    public virtual void When_detect_pressed(Vector2 coords) { }
    public virtual void When_detect_released(Vector2 coords) { }
}
