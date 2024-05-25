class_name AllyAttack
extends AllyState

@onready var attack_cooldown: Timer = $Cooldown

var shot: CharacterBody2D = null

func enter() -> void:
	if ally.name.contains("Fighter"):
		ally.time_between_hits = 1.5
		ally.damage = 20
	elif ally.name.contains("Artist"):
		ally.time_between_hits = 1.0
		ally.damage = 20
	pass

func exit() -> void:
	pass

func update(_delta: float) -> void:
	if ally.target_number == 0:
		change_state("Idle")
	elif attack_cooldown.is_stopped():
		var dead_targets: Array = ally.targets.filter(func(target): return !is_instance_valid(target))
		for target in dead_targets:
			ally.targets.erase(target)
		ally.target_number = ally.targets.size()
		if ally.target_number > 0:
			if ally.name.contains("Fighter"):
				shot = ally.shot_fighters.instantiate()
			elif ally.name.contains("Artist"):
				shot = ally.shot_artists.instantiate()
			ally.add_child(shot)
			shot.position.y = position.y - 60
			attack_cooldown.start(ally.time_between_hits)
	pass

func _on_detector_body_entered(body):
	if body is Enemy:
		ally.targets.append(body)
		ally.current_target = ally.targets[0]
		ally.target_number = ally.targets.size()

func _on_detector_body_exited(body):
	if body is Enemy:
		ally.targets.erase(body)
		ally.target_number = ally.targets.size()
		if ally.target_number > 0:
			ally.current_target = ally.targets[0]
		elif state_machine.current_state is AllyAttack:
			change_state("Idle")
