class_name TowerShot
extends CharacterBody2D

@onready var tower: Tower = get_parent()
var shot_speed: float = 200

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	var target_pos = tower.current_target.global_position
	look_at(target_pos)
	if target_pos.x < global_position.x:
		velocity.x = -shot_speed
	elif target_pos.x > global_position.x:
		velocity.x = shot_speed
	else:
		velocity.x = 0
	if target_pos.y < global_position.y:
		velocity.y = -shot_speed
	elif target_pos.y > global_position.y:
		velocity.y = shot_speed
	else:
		velocity.y = 0
	move_and_slide()
	pass

func _on_hitbox_body_entered(body):
	if body.is_in_group("Enemy"):
		queue_free()
	pass # Replace with function body.
