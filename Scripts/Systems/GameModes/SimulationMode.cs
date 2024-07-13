using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class SimulationMode : GameMode {

    public Area2D selection_area;
    public RectangleShape2D selection_shape;
    public CollisionShape2D selection_polygon;
    public SelectionBox visual_selection;

    public bool dragging;
    public bool interacted_with_building;
    
    public Vector2 starting_point;
    public Vector2 shape_size;
    public Vector2 shape_position;
    public Array<Ally> selected_allies = new();
    public Building building_to_interact;
    
    public override void Enter() {  }
    public override void Exit() {  }

    public override void Update() {
        if (dragging) {
            shape_size = mode_manager.current_level.GetGlobalMousePosition() - starting_point;
            shape_position = shape_size / 2;
            visual_selection.selection_shape.Size = shape_size.Abs();
            selection_polygon.Position = shape_position;
            visual_selection.Position = shape_position;
            visual_selection.QueueRedraw();
        }
    }
    public override void When_detect_pressed(Vector2 coords) {
        starting_point = coords;
        selection_area = new Area2D();
        selection_polygon = new CollisionShape2D();
        selection_shape = new RectangleShape2D();
        visual_selection = new SelectionBox();
        selection_polygon.Shape = selection_shape;
        selection_area.SetCollisionMask(2);
        visual_selection.selection_shape = selection_shape;
        selection_area.AddChild(selection_polygon);
        selection_area.AddChild(visual_selection);
        AddChild(selection_area);
        selection_area.GlobalPosition = starting_point;
        dragging = true;
    }

    public override void When_detect_released(Vector2 coords) {
        var overlapping_bodies = selection_area.GetOverlappingBodies();
        foreach (var ally in selected_allies){
            ally.currently_selected = false;
        }
        selected_allies.Clear();
        if (overlapping_bodies.Count != 0){
            foreach (var body in overlapping_bodies){
                if (body is Ally ally){
                    selected_allies.Add(ally);
                    ally.currently_selected = true;
                }
            }
        }
        dragging = false;
        selection_area.QueueFree();
        visual_selection.QueueFree();
    }

    public void When_about_to_interact_with_building(Building building){
        building_to_interact = building;
    }
    
    public void When_interaction_with_building_removed(){
        building_to_interact = null;
    }
    
}
