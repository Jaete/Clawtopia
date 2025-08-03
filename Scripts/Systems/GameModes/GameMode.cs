using System;
using Godot;

public partial class GameMode : Node2D
{
    [Signal]
    public delegate void BuildCompletedEventHandler(Building building);

    [Signal]
    public delegate void ConstructionStartedEventHandler(Building building);

    [Signal]
    public delegate void ModeTransitionEventHandler(string mode, Building building);


    public const string BUILD_MODE = "BuildMode";
    public const string SIMULATION_MODE = "SimulationMode";
    public const string UI_MODE = "UIMode";

    public override void _Ready()
    {
        Initialize();
    }

    public virtual void Enter() {}

    public virtual void Exit() {}

    public virtual void Update() {}

    public virtual void MousePressed(Vector2 coords) {}

    public virtual void MouseReleased(Vector2 coords) {}

    public virtual void MouseRightPressed(Vector2 coords) {}

    public virtual void RotateBuilding() {}

    public void Initialize()
    {
    }
}