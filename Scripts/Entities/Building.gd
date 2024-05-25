class_name Building
extends Area2D

@onready var static_body: StaticBody2D = $NavigationBody
@onready var region: NavigationRegion2D = $"../NavigationRegion2D"
@onready var director: Director = $".."

var building: Node2D
var is_pre_spawned: bool = false
var self_index: int = 0
var already_baked: Array[int]
var in_build_mode: bool
var placed: bool

func _ready():
	pass

func set_rebake():
	static_body.set_name(str("Obstacle_Region_", self_index))
	static_body.reparent(region)
	region.bake_navigation_polygon(true)
	already_baked.erase(self_index)
	director.currently_baking = true

func rebake_add_building():
	if director.current_mode is BuildMode:
		set_rebake()
		return
	if is_pre_spawned:
		if !director.currently_baking:
			set_rebake()
		else:
			rebake_add_building()

func rebake_remove_building():
	if !director.currently_baking:
		var obstacle: StaticBody2D = region.get_node(str("Obstacle_Region_", self_index))
		obstacle.reparent(building)
		region.bake_navigation_polygon(true)
		director.currently_baking = true
	else:
		rebake_remove_building()

func free_to_rebake():
	director.currently_baking = false
	placed = true

func _on_input_event(viewport, event, shape_idx):
	if director.current_mode is SimulationMode:
		if event is InputEventMouseButton:
			if event.pressed && event.button_index == MOUSE_BUTTON_LEFT && placed:
				rebake_remove_building()
				queue_free()
	pass # Replace with function body.
