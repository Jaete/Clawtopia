class_name AllyIdle
extends AllyState

func enter() -> void:
	pass

func exit() -> void:
	pass

func update(_delta: float) -> void:
	pass

func _on_detector_body_entered(body):
	if state_machine.current_state is AllyIdle && body is Enemy:
		ally.targets.append(body)
		ally.current_target = ally.targets[0]
		ally.target_number = ally.targets.size()
		change_state("Attack")
