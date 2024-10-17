using Godot;
using Godot.Collections;

public partial class SimulationMode : GameMode
{
    // RELACIONADO PARA SELECAO EM AREA
    public Area2D SelectionArea;
    public RectangleShape2D SelectionShape;
    public CollisionShape2D SelectionPolygon;
    public SelectionBox VisualSelection;
    public Vector2 StartingPoint;
    public Vector2 ShapeSize;
    public Vector2 ShapePosition;
    public bool Dragging;
    
    // RELACIONADO PARA REFERENCIA DE UNIDADES SELECIONADAS
    public Array<Ally> SelectedAllies = new();
    public Array<Building> SelectedBuildings = new();
    public Building BuildingToInteract;
    public bool InteractedWithBuilding;
    
    // RELACIONADO PARA INTERACAO COM RECURSOS
    public bool IsWater;
    public TileMap TileMap;

    public override void _Ready(){
        Initialize();
        TileMap = GetNode<TileMap>("/root/Game/LevelManager/Level/Navigation/TileMap");
    }

    public override void Enter() {  }
    public override void Exit() {  }

    public override void Update() {
        if (Dragging) {
            ShapeSize = ModeManager.CurrentLevel.GetGlobalMousePosition() - StartingPoint;
            ShapePosition = ShapeSize / 2;
            VisualSelection.SelectionShape.Size = ShapeSize.Abs();
            SelectionPolygon.Position = ShapePosition;
            VisualSelection.Position = ShapePosition;
            VisualSelection.QueueRedraw();
        }
    }
    public override void MousePressed(Vector2 coords) {
        if (ModeManager.CurrentMode is not SimulationMode){
            return;
        }
        StartingPoint = coords;
        SelectionArea = new Area2D();
        SelectionPolygon = new CollisionShape2D();
        SelectionShape = new RectangleShape2D();
        VisualSelection = new SelectionBox();
        SelectionPolygon.Shape = SelectionShape; 
        VisualSelection.SelectionShape = SelectionShape;
        SelectionArea.AddChild(SelectionPolygon);
        SelectionArea.AddChild(VisualSelection);
        AddChild(SelectionArea);
        SelectionArea.GlobalPosition = StartingPoint;
        Dragging = true;
    }

    public override void MouseReleased(Vector2 coords){
        if (ModeManager.CurrentMode is not SimulationMode){
            return;
        }
        if (VisualSelection.SelectionShape.Size.X < 2 && VisualSelection.SelectionShape.Size.Y < 2){
            Select_units(true);
            return;
        }
        Select_units();
    }

    public void AboutToInteractWithBuilding(Building building){
        BuildingToInteract = building;
    }
    
    public void InteractionWithBuildingRemoved(){
        BuildingToInteract = null;
    }

    public void Select_units(bool treatAsClick = false){
        UI ui = GetNode<UI>("/root/Game/UI");
        var overlappingAreas = SelectionArea.GetOverlappingAreas();
        if (overlappingAreas.Count == 0){
            ui.Reset_ui();
            Erase_selection_box();
            Clear_selected_allies();
            return;
        }
        if (!Input.IsActionPressed("Multiple")){
            Clear_selected_allies();
        }
        if(treatAsClick){
            var areaInFront = Select_top_unit(overlappingAreas);
            if (areaInFront is Building building){
                SelectedBuildings.Add(building);
                ui.Instantiate_window(Constants.BUILDING_MENU, building);
            }
            if (areaInFront.GetParent() is Ally ally){
                SelectedAllies.Add(ally);
                ally.CurrentlySelected = true;
                var selectionCircle = ally.GetNode<Line2D>("SelectionCircle");
                selectionCircle.Visible = true;
                ui.Instantiate_window(Constants.COMMUNIST_MENU);
            }
        }else{
            foreach (var area in overlappingAreas){
                if (area.GetParent() is Ally ally){
                    SelectedAllies.Add(ally);
                    ally.CurrentlySelected = true;
                    var selectionCircle = ally.GetNode<Line2D>("SelectionCircle");
                    selectionCircle.Visible = true;
                }
            }
            ui.Instantiate_window(Constants.COMMUNIST_MENU);
        }
        Erase_selection_box();
    }
    
    public Area2D Select_top_unit(Array<Area2D> overlappingAreas){
        var areaInFront = new Area2D();
        foreach (var area in overlappingAreas){
            if (area.GlobalPosition.Y > areaInFront.GlobalPosition.Y){
                areaInFront = area;
            }
        }
        return areaInFront;
    }

    public void Erase_selection_box(){
        Dragging = false;
        SelectionArea.QueueFree();
        VisualSelection.QueueFree();
    }

    public void Clear_selected_allies(){
        foreach (var ally in SelectedAllies){
            ally.CurrentlySelected = false;
            var selectionCircle = ally.GetNode<Line2D>("SelectionCircle");
            selectionCircle.Visible = false;
        }
        SelectedAllies.Clear();
    }
}


