class_name NavigationObstacleStructure
extends CollisionPolygon2D

@onready var static_body: StaticBody2D = $".."
@onready var region: NavigationRegion2D = $"../../../NavigationRegion2D"
@onready var director: Director = get_node("/root/SceneManager/Director")

var tower: Node2D
var self_index: int = 0
var already_baked: Array[int]
var in_build_mode: bool

func _ready():
	tower = director.get_node(str(director.tower_type + "_T1(" + str(director.tower_count) + ")"))
	pass

func bake_init():
	in_build_mode = director.current_mode is BuildMode
	if !in_build_mode:
		region.bake_finished.connect(free_to_rebake)
		self_index += 1
		already_baked.append(self_index)

func rebake_add_tower():
	self_index = director.tower_count
	static_body.set_name(str("Obstacle_Region_", self_index))
	static_body.reparent(region)
	region.bake_navigation_polygon(true)
	already_baked.erase(self_index)
	director.currently_baking = true

func rebake_remove_tower():
	var obstacle: StaticBody2D = region.get_node(str("Obstacle_Region_", self_index))
	if !director.currently_baking:
		obstacle.reparent(tower)
		region.bake_navigation_polygon(true)
		director.currently_baking = true
	else:
		rebake_remove_tower()

func free_to_rebake():
	director.currently_baking = false


func _on_navigation_body_input_event(viewport, event, shape_idx):
	if event.is_action_just_pressed("LeftClick"):
		rebake_remove_tower()
	pass # Replace with function body.
