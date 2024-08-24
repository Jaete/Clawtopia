using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class SimulationMode : GameMode
{
    // RELACIONADO PARA SELECAO EM AREA
    public Area2D selection_area;
    public RectangleShape2D selection_shape;
    public CollisionShape2D selection_polygon;
    public SelectionBox visual_selection;
    public Vector2 starting_point;
    public Vector2 shape_size;
    public Vector2 shape_position;
    public bool dragging;
    
    // RELACIONADO PARA REFERENCIA DE UNIDADES SELECIONADAS
    public Array<Ally> selected_allies = new();
    public Array<Building> selected_buildings = new();
    public Building building_to_interact;
    public bool interacted_with_building;
    
    // RELACIONADO PARA INTERACAO COM RECURSOS
    public bool is_water;
    public TileMap tile_map;

    public override void _Ready(){
        Initialize();
        tile_map = GetNode<TileMap>("/root/Game/LevelManager/Level/Navigation/TileMap");
    }

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
        if (mode_manager.current_mode is not SimulationMode){
            return;
        }
        starting_point = coords;
        selection_area = new Area2D();
        selection_polygon = new CollisionShape2D();
        selection_shape = new RectangleShape2D();
        visual_selection = new SelectionBox();
        selection_polygon.Shape = selection_shape; 
        visual_selection.selection_shape = selection_shape;
        selection_area.AddChild(selection_polygon);
        selection_area.AddChild(visual_selection);
        AddChild(selection_area);
        selection_area.GlobalPosition = starting_point;
        dragging = true;
    }

    public override void When_detect_released(Vector2 coords){
        if (mode_manager.current_mode is not SimulationMode){
            return;
        }
        if (visual_selection.selection_shape.Size.X < 2 && visual_selection.selection_shape.Size.Y < 2){
            Select_units(true);
            return;
        }
        Select_units();
    }

    public void When_about_to_interact_with_building(Building building){
        building_to_interact = building;
    }
    
    public void When_interaction_with_building_removed(){
        building_to_interact = null;
    }

    public void Select_units(bool treat_as_click = false){
        UI ui = GetNode<UI>("/root/Game/UI");
        var overlapping_areas = selection_area.GetOverlappingAreas();
        if (overlapping_areas.Count == 0){
            ui.Reset_ui();
            Erase_selection_box();
            Clear_selected_allies();
            return;
        }
        if (!Input.IsActionPressed("Multiple")){
            Clear_selected_allies();
        }
        if(treat_as_click){
            var area_in_front = Select_top_unit(overlapping_areas);
            if (area_in_front is Building building){
                selected_buildings.Add(building);
                ui.Instantiate_window(Constants.BUILDING_MENU, building);
            }
            if (area_in_front.GetParent() is Ally ally){
                selected_allies.Add(ally);
                ally.currently_selected = true;
                var selection_circle = ally.GetNode<Line2D>("SelectionCircle");
                selection_circle.Visible = true;
                ui.Instantiate_window(Constants.COMMUNIST_MENU);
            }
        }else{
            foreach (var area in overlapping_areas){
                if (area.GetParent() is Ally ally){
                    selected_allies.Add(ally);
                    ally.currently_selected = true;
                    var selection_circle = ally.GetNode<Line2D>("SelectionCircle");
                    selection_circle.Visible = true;
                }
            }
            ui.Instantiate_window(Constants.COMMUNIST_MENU);
        }
        Erase_selection_box();
    }
    
    public Area2D Select_top_unit(Array<Area2D> overlapping_areas){
        var area_in_front = new Area2D();
        foreach (var area in overlapping_areas){
            if (area.GlobalPosition.Y > area_in_front.GlobalPosition.Y){
                area_in_front = area;
            }
        }
        return area_in_front;
    }

    public void Erase_selection_box(){
        dragging = false;
        selection_area.QueueFree();
        visual_selection.QueueFree();
    }

    public void Clear_selected_allies(){
        foreach (var ally in selected_allies){
            ally.currently_selected = false;
            var selection_circle = ally.GetNode<Line2D>("SelectionCircle");
            selection_circle.Visible = false;
        }
        selected_allies.Clear();
    }
}


