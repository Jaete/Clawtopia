class_name EnemyMove
extends EnemyState

func enter(target: Variant) -> void:
	set_enemy_target(target)
	pass

func update(_delta: float) -> void:
	if navigation.is_navigation_finished():
		change_state("Idle")
	var path_position: Vector2 = navigation.get_next_path_position()
	var new_direction: Vector2 = enemy.global_position.direction_to(path_position)
	navigation.set_velocity(new_direction)
	pass

func exit() -> void:
	pass

func set_enemy_target(target: Vector2):
	navigation.set_target_position(target)

func _on_navigation_agent_2d_velocity_computed(safe_velocity):
	enemy.velocity = safe_velocity * enemy.speed
	enemy.move_and_slide()
	pass
