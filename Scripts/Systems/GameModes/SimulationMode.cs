using ClawtopiaCs.Scripts.Entities.Building;
using Godot;
using Godot.Collections;

namespace ClawtopiaCs.Scripts.Systems.GameModes;

public partial class SimulationMode : GameMode
{
    public Array<Building> BuildingsToInteract = new();

    [Export] public Label Debug;
    public bool Dragging;
    public TileMapLayer Ground;
    public bool InteractedWithBuilding;

    // RELACIONADO PARA INTERACAO COM RECURSOS
    public bool IsWater;

    // RELACIONADO PARA REFERENCIA DE UNIDADES SELECIONADAS
    public Array<Ally> SelectedAllies = new();

    public Building SelectedBuilding;

    // RELACIONADO PARA SELECAO EM AREA
    public Area2D SelectionArea;
    public CollisionShape2D SelectionPolygon;
    public RectangleShape2D SelectionShape;
    public Vector2 ShapePosition;
    public Vector2 ShapeSize;
    public Vector2 StartingPoint;
    public SelectionBox VisualSelection;
    public TileMapLayer Water;

    public override void _Ready()
    {
        Initialize();
        Ground = GetNode<TileMapLayer>("/root/Game/LevelManager/Level/Navigation/Ground");
        Water = GetNode<TileMapLayer>("/root/Game/LevelManager/Level/Navigation/Water");
        Debug.Text = $"Selected Allies: {SelectedAllies.Count}";
    }

    public override void Enter() { }
    public override void Exit() { }

    public override void Update()
    {
        if (Dragging) {
            ShapeSize = ModeManager.CurrentLevel.GetGlobalMousePosition() - StartingPoint;
            ShapePosition = ShapeSize / 2;
            VisualSelection.SelectionShape.Size = ShapeSize.Abs();
            SelectionPolygon.Position = ShapePosition;
            VisualSelection.Position = ShapePosition;
            VisualSelection.QueueRedraw();
        }
    }

    public override void MousePressed(Vector2 coords)
    {
        if (ModeManager.CurrentMode is not SimulationMode) {
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

    public override void MouseReleased(Vector2 coords)
    {
        if (ModeManager.CurrentMode is not SimulationMode) {
            return;
        }

        var hasBuildings = BuildingsToInteract.Count > 0;
        var treatAsClick = VisualSelection.SelectionShape.Size is { X: < 2, Y: < 2 };
        SelectEntities(treatAsClick, hasBuildings);
        Debug.Text = $"Selected Allies: {SelectedAllies.Count}";
    }

    public void AboutToInteractWithBuilding(Building building)
    {
        if (!BuildingsToInteract.Contains(building)) {
            BuildingsToInteract.Add(building);
        }

        if (BuildingsToInteract[0] != building && building.GlobalPosition.Y > BuildingsToInteract[0].GlobalPosition.Y) {
            var lastBuildingInteracted = BuildingsToInteract[0];
            BuildingsToInteract[0] = building;
            BuildingsToInteract[^1] = lastBuildingInteracted;
        }

        Building.ModulateBuilding(BuildingsToInteract[0], BuildingInteractionStates.HOVER);
        Building.ModulateBuilding(BuildingsToInteract[^1], BuildingInteractionStates.UNHOVER);
    }

    public void InteractionWithBuildingRemoved(Building building)
    {
        if (BuildingsToInteract.Contains(building)) {
            BuildingsToInteract.Remove(building);
        }

        Building.ModulateBuilding(building, BuildingInteractionStates.UNHOVER);

        if (BuildingsToInteract.Count > 0) {
            Building.ModulateBuilding(BuildingsToInteract[0], BuildingInteractionStates.HOVER);
        }
    }

    public void SelectEntities(bool treatAsClick, bool hasBuildings)
    {
        var overlappingAreas = SelectionArea.GetOverlappingAreas();
        var ui = GetNode<UI>("/root/Game/UI");
        if (overlappingAreas.Count == 0) {
            ui.Reset_ui();
            Selectors.ClearSelectedAllies(SelectedAllies);
            SelectedBuilding = null;
            EraseSelectionBox();
            return;
        }

        if (treatAsClick) {
            if (hasBuildings) {
                Selectors.SelectSingleBuilding(overlappingAreas, ui);
            }
            else {
                if (SelectedAllies.Count > 0 && !Input.IsActionPressed("Multiple")) {
                    Selectors.ClearSelectedAllies(SelectedAllies);
                }

                SelectedAllies.Add(Selectors.SelectSingleUnit(overlappingAreas, ui));
            }
        }
        else {
            if (SelectedAllies.Count > 0 && !Input.IsActionPressed("Multiple")) {
                Selectors.ClearSelectedAllies(SelectedAllies);
                SelectedAllies = Selectors.SelectMultipleUnits(overlappingAreas);
            }
            else {
                SelectedAllies = Selectors.SelectMultipleUnits(overlappingAreas, SelectedAllies);
            }
        }

        if (SelectedAllies.Count > 0) {
            ui.Instantiate_window(Constants.COMMUNIST_MENU);
        }

        EraseSelectionBox();
    }

    private void SelectUnits(bool treatAsClick)
    {
        if (!IsInstanceValid(SelectionArea)) {
            return;
        }

        var ui = GetNode<UI>("/root/Game/UI");
        var overlappingAreas = SelectionArea.GetOverlappingAreas();
        if (overlappingAreas is null) {
            return;
        }

        if (overlappingAreas.Count == 0) {
            ui.Reset_ui();
            Selectors.ClearSelectedAllies(SelectedAllies);
            return;
        }

        if (!Input.IsActionPressed("Multiple")) {
            Selectors.ClearSelectedAllies(SelectedAllies);
        }

        if (treatAsClick) {
            var unit = Selectors.SelectSingleUnit(overlappingAreas, ui);
            if (unit is null) {
                return;
            }

            SelectedAllies.Add(unit);
        }
        else {
            SelectedAllies = Selectors.SelectMultipleUnits(overlappingAreas);
        }
    }

    public void EraseSelectionBox()
    {
        Dragging = false;
        SelectionArea.QueueFree();
        VisualSelection.QueueFree();
    }
}