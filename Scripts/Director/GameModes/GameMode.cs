using Godot;
using System;

public partial class GameMode : Node {
    [Signal]
    public delegate void ModeTransitionEventHandler(String mode, String building, String type);
    [Signal]
    public delegate void MouseClickedEventHandler();

    public Director director;
    public Node2D current_level;
    public void initialize() {
        director = (Director)GetNode("/root/Game/Director");
        current_level = (Node2D)GetNode("/root/Game/SceneManager/Level");
    }
    public virtual void enter() { }
    public virtual void exit() { }
    public virtual void update() { }
}
