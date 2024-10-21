using Godot;
using System;

public partial class GameMode : Node2D {
    [Signal]
    public delegate void ModeTransitionEventHandler(String mode, String building, String type);

    [Signal]
    public delegate void ConstructionStartedEventHandler(Building building);
    [Signal]
    public delegate void BuildCompletedEventHandler(Building building);

    public Director Director;
    public Node2D CurrentLevel;

    public ModeManager ModeManager;
    
    public Controller Controller;

    public Rid MapRid;

    public override void _Ready(){
        Initialize();
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }

    public virtual void MousePressed(Vector2 coords) { }
    public virtual void MouseReleased(Vector2 coords) { }

    public void Initialize(){
        //Director = GetNode<Director>("/root/Game/Director");
        ModeManager = GetParent<ModeManager>();
        Controller = GetNode<Controller>("/root/Game/Controller");
        Controller.MousePressed += MousePressed;
        Controller.MouseReleased += MouseReleased;
    }
}

