class_name TowerIdle
extends TowerState



func _on_enemy_detection_body_entered(body):
	if state_machine.current_state.name == self.name && body.is_in_group("Enemy"):
		change_state("Attack")
	pass
