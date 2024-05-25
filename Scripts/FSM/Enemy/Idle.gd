class_name EnemyIdle
extends EnemyState

@onready var initial_target: Building = get_node("/root/SceneManager/Director/AllyBase")

func enter(generic_param: Variant) -> void:
	pass

func update(_delta: float) -> void:
	if Input.is_action_just_released("LeftClick"):
		change_state("Move", initial_target.global_position)
	pass

func exit() -> void:
	pass
