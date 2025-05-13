using ClawtopiaCs.Scripts.Entities.Building;
using Godot;
using Godot.Collections;

namespace ClawtopiaCs.Scripts.Systems.GameModes;

public partial class SimulationMode : GameMode
{
    [Signal] public delegate void AllyCommandEventHandler(Vector2 coords);

    public static SimulationMode Singleton { get; private set; }

    public Array<Building> BuildingsToInteract = new();
    public TileMapLayer Bushes;

    [Export] public Label Debug;
    public bool Dragging;
    public TileMapLayer Ground;
    public bool InteractedWithBuilding;

    // RELACIONADO PARA INTERACAO COM RECURSOS
    public bool IsWater;
    public TileMapLayer SandDumps;

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

    public override void _EnterTree()
    {
        Singleton = this;
    }

    public override void _Ready()
    {
        Initialize();
        Debug.Text = $"Selected Allies: {SelectedAllies.Count}";
    }

    public override void Enter() {}

    public override void Exit() {}

    public override void Update()
    {
        if (Dragging)
        {
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
        if (!IsInstanceValid(SelectionArea)) { return; }
        var hasBuildings = BuildingsToInteract.Count > 0;
        var treatAsClick = VisualSelection.SelectionShape?.Size is { X: < 2, Y: < 2 };
        SelectEntities(treatAsClick, hasBuildings);
        Debug.Text = $"Selected Allies: {SelectedAllies.Count}";
    }

    public override void MouseRightPressed(Vector2 coords)
    {
        if (SelectedAllies.Count == 0) { return; }

        EmitSignal(SignalName.AllyCommand, coords);
    }

    public void AboutToInteractWithBuilding(Building building)
    {
        if (!BuildingsToInteract.Contains(building))
        {
            BuildingsToInteract.Add(building);
        }

        if (BuildingsToInteract[0] != building && building.GlobalPosition.Y > BuildingsToInteract[0].GlobalPosition.Y)
        {
            var lastBuildingInteracted = BuildingsToInteract[0];
            BuildingsToInteract[0] = building;
            BuildingsToInteract[^1] = lastBuildingInteracted;
        }

        Building.ModulateBuilding(BuildingsToInteract[0], BuildingInteractionStates.HOVER);

        if(BuildingsToInteract.Count > 1)
        {
            Building.ModulateBuilding(BuildingsToInteract[^1], BuildingInteractionStates.UNHOVER);
        }
    }

    public void InteractionWithBuildingRemoved(Building building)
    {
        if (BuildingsToInteract.Contains(building))
        {
            BuildingsToInteract.Remove(building);
        }

        Building.ModulateBuilding(building, BuildingInteractionStates.UNHOVER);

        if (BuildingsToInteract.Count > 0)
        {
            Building.ModulateBuilding(BuildingsToInteract[0], BuildingInteractionStates.HOVER);
        }
    }

    public void SelectEntities(bool treatAsClick, bool hasBuildings)
    {
       
        var overlappingAreas = SelectionArea.GetOverlappingAreas();
        var ui = GetNode<UI>("/root/Game/UI");

        if (overlappingAreas.Count == 0)
        {
            ui.Reset_ui();
            Selectors.ClearSelectedAllies(SelectedAllies);
            SelectedBuilding = null;
            EraseSelectionBox();
            return;
        }

        if (treatAsClick)
        {
            if (SelectedAllies.Count > 0 && !Input.IsActionPressed("Multiple"))
            {
                Selectors.ClearSelectedAllies(SelectedAllies);
            }
            if (hasBuildings)
            {
                Selectors.SelectSingleBuilding(overlappingAreas, ui);
            }
            else
            {
                SelectedAllies.Add(Selectors.SelectSingleUnit(overlappingAreas, ui));
            }
        }
        else
        {
            if (SelectedAllies.Count > 0 && !Input.IsActionPressed("Multiple"))
            {
                Selectors.ClearSelectedAllies(SelectedAllies);
            }
            SelectedAllies = Selectors.SelectMultipleUnits(overlappingAreas, SelectedAllies);
        }

        if (SelectedAllies.Count > 0)
        {
            ui.Instantiate_window(Constants.COMMUNIST_MENU);
        }

        EraseSelectionBox();
    }

    public void EraseSelectionBox()
    {
        Dragging = false;
        SelectionArea.QueueFree();
        VisualSelection.QueueFree();
    }
}