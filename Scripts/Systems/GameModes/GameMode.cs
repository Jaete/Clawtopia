using System;
using Godot;

public partial class GameMode : Node2D
{
    [Signal]
    public delegate void BuildCompletedEventHandler(Building building);

    [Signal]
    public delegate void ConstructionStartedEventHandler(Building building);

    [Signal]
    public delegate void ModeTransitionEventHandler(String mode, String building, String type);

    public enum Modes
    {
        BUILD_MODE = 0,
        SIMULATION_MODE,
    }

    public Controller Controller;
    public Node2D CurrentLevel;

    public Director Director;

    public Rid MapRid;

    public ModeManager ModeManager;

    public override void _Ready()
    {
        Initialize();
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }

    public virtual void MousePressed(Vector2 coords) { }
    public virtual void MouseReleased(Vector2 coords) { }

    public virtual void MouseRightPressed(Vector2 coords) { }

    public void Initialize()
    {
        //Director = GetNode<Director>("/root/Game/Director");
        ModeManager = GetParent<ModeManager>();
    }
}