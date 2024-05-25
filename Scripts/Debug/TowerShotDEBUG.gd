extends Label

@onready var shot: CharacterBody2D = get_parent()
@onready var tower: Tower = get_parent().get_parent()
var shot_pos: Vector2
var target_pos: Vector2
func _physics_process(delta):
	if is_instance_valid(tower.current_target):
		target_pos = tower.current_target.global_position
	
