class_name Director
extends Node2D

var currently_baking: bool = false


var game_modes: Dictionary = {}
var current_mode: GameMode

var building_type: String
var building_count: int = 0

var tower_type: String = ""
var tower_count: int = 0

var resource_build_type: String = ""
var resource_build_count: int = 0

func _ready():
	var modes: Array[Node] = get_children()
	for mode in modes:
		if mode is GameMode:
			game_modes[mode.name] = mode
			game_modes[mode.name].mode_transition.connect(enter_mode)
	current_mode = game_modes["SimulationMode"]
	pass

func enter_mode(mode: String, building: String = "", type: String = ""):
	building_type = building
	tower_type = type
	current_mode.exit()
	current_mode = game_modes[mode]
	current_mode.enter()
	pass

func _process(delta):
	current_mode.update()
	pass

