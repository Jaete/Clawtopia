using Godot;
using System;
using System.Linq;

public partial class SimulationMode : GameMode {

    public Area2D selection_area;
    public RectangleShape2D selection_shape;
    public CollisionShape2D selection_polygon;
    public SelectionBox visual_selection;

    public bool dragging = false;
    public Vector2 starting_point;
    public Vector2 shape_size;
    public Vector2 shape_position;
    
    public override void Enter() {  }
    public override void Exit() {  }

    public override void Update() {
        if (dragging) {
            shape_size = mode_manager.current_level.GetGlobalMousePosition() - starting_point;
            shape_position = shape_size / 2;
            selection_shape.Size = shape_size.Abs();
            selection_polygon.Position = shape_position;
            visual_selection.selection_shape = this.selection_shape;
            visual_selection.Position = shape_position;
            if(IsInstanceValid(visual_selection)){
                visual_selection.QueueRedraw();
            }
        }
    }
    public override void Detect_pressed(Vector2 coords) {
        starting_point = coords;
        dragging = true;
        selection_area = new Area2D();
        selection_polygon = new CollisionShape2D();
        selection_shape = new RectangleShape2D();
        visual_selection = new SelectionBox();
        selection_polygon.Shape = selection_shape;
        selection_area.AddChild(selection_polygon);
        selection_area.AddChild(visual_selection);
        AddChild(selection_area);
        selection_area.GlobalPosition = starting_point;
    }

    public override void Detect_released(Vector2 coords) {
        //Godot.Collections.Array<Ally> allies = new Godot.Collections.Array<Ally>();
        Godot.Collections.Array<Node2D> overlapping_bodies = selection_area.GetOverlappingBodies();
        if (overlapping_bodies.Count != 0) {
            foreach (var body in overlapping_bodies) {
                if(body is Ally ally) {
                    director.selected_allies.Add(ally);
                }
            }
        }else{
            director.selected_allies.Clear();
        }
        dragging = false;
        selection_area.QueueFree();
        visual_selection.QueueFree();
    }

    public override void _Input(InputEvent @event) {
        var eventButton = @event as InputEventMouseButton;
        if (eventButton != null && eventButton.Pressed && eventButton.ButtonIndex == MouseButton.Left) {
           EmitSignal("MousePressed", mode_manager.current_level.GetGlobalMousePosition());
        }
        if (eventButton != null && !eventButton.Pressed && eventButton.ButtonIndex == MouseButton.Left) {
            EmitSignal("MouseReleased", mode_manager.current_level.GetGlobalMousePosition());
        }
    }

    
}
