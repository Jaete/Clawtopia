class_name BuildMode
extends GameMode


@onready var director: Director = $".."

const OK_COLOR: Color = Color("2eff3f81")
const ERROR_COLOR: Color = Color("ba000079")
const REGULAR_COLOR: Color = Color(1,1,1,1)

const TILE_SIZE_X: int = 48
const TILE_SIZE_Y: int = 24
var mouse_position: Vector2

var building: Building
var is_overlapping_buildings: bool = false

func enter():
	match(director.building_type):
		"Tower":
			building = load("res://TSCN/Objects/Buildings/Towers/" + director.tower_type + ".tscn").instantiate()
			director.building_count += 1
			director.tower_count += 1
			building.set_name(director.tower_type + "_T1(" + str(director.tower_count) + ")")
		"Base":
			building = load("res://TSCN/Objects/Buildings/Base/GreatCommune.tscn").instantiate()
			director.building_count += 1
			building.set_name("AllyBase")
		"Resource":
			building = load("res://TSCN/Objects/Buildings/Resource/"+ director.resource_type + ".tscn").instantiate()
			director.resource_build_count += 1
			building.set_name(str("", director.resource_build_type, "_", director.resource_build_count))
	director.add_child(building)
	mouse_position = director.get_global_mouse_position()
	building.global_position = mouse_position
	pass

func update():
	move_preview()
	validate_position()
	if Input.is_action_pressed("LeftClick"):
		clicked_while_building()
	if Input.is_action_just_pressed("RightClick"):
		right_click()
	pass

func exit():
	pass

func move_preview():
	mouse_position = director.get_global_mouse_position()
	var x_difference: float =  mouse_position.x - building.global_position.x
	var y_difference: float = mouse_position.y - building.global_position.y
	var new_x: float
	var new_y: float
	if x_difference > (TILE_SIZE_X / 2):
		new_x = (TILE_SIZE_X / 2)
	elif x_difference < -(TILE_SIZE_X / 2):
		new_x = -(TILE_SIZE_X / 2)
	if y_difference > (TILE_SIZE_Y / 2):
		new_y = (TILE_SIZE_Y / 2)
	elif y_difference < -(TILE_SIZE_Y / 2):
		new_y = -(TILE_SIZE_Y / 2)
	building.global_position = Vector2(
		building.global_position.x + new_x,
		building.global_position.y + new_y
	)

func validate_position():
	var grid_area: Area2D = building.get_node("GridArea")
	var overlapping_areas: Array[Area2D] = grid_area.get_overlapping_areas()
	var other_buildings: int = 0
	for area in overlapping_areas:
		if area.name == "GridArea":
			other_buildings += 1
	if other_buildings > 0:
		is_overlapping_buildings = true
	else:
		is_overlapping_buildings = false
	if is_overlapping_buildings:
		building.get_node("Sprite").set_modulate(ERROR_COLOR)
	else:
		building.get_node("Sprite").set_modulate(OK_COLOR)

func clicked_while_building():
	if !is_overlapping_buildings:
		building.rebake_add_building()
		building.get_node("Sprite").set_modulate(REGULAR_COLOR)
		mode_transition.emit("SimulationMode")
	pass

func right_click():
	building.queue_free()
	mode_transition.emit("SimulationMode")
