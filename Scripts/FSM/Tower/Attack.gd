class_name TowerAttack
extends TowerState

@onready var attack_cooldown: Timer = $AttackCooldown
@onready var tower: Tower = $"../.."
var time_between_hits: float

func enter():
	logger_divider("ASSIGNING ENEMIES TO LIST")
	var targets: Array[Node] = get_tree().get_nodes_in_group("Enemy") 
	for target in targets:
		tower.targets.append(target)
		print("Target: " + target.name)
	logger_divider("TARGET LIST READY")

func exit():
	pass

func update(_delta: float):
	if attack_cooldown.is_stopped():
		var shot = tower_shot.instantiate()
		tower.add_child(shot)
		attack_cooldown.start(time_between_hits)
	pass
