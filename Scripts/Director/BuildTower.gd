class_name BuildTower
extends Button

@onready var window: Window = get_node("/root")
@onready var director: Director = $"../../../"

func _on_pressed():
	var tower_type: String
	var building_type: String = "Tower"
	if self.name.contains("Artist"): tower_type = "Artists"
	if self.name.contains("Fighter"): tower_type = "Fighters"
	director.current_mode.mode_transition.emit("BuildMode", building_type, tower_type)
	pass
