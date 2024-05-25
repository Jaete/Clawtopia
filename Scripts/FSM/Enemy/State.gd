class_name EnemyState
extends Node

signal state_transition()

@onready var state_machine: EnemyStateMachine = get_parent()
@onready var enemy: Enemy = $"../.."
@onready var navigation: NavigationAgent2D = $"../../NavigationAgent2D"

func enter(generic_param: Variant) -> void:
	pass

func update(_delta: float) -> void:
	pass

func exit() -> void:
	pass

func change_state(state: String, generic_param: Variant = null) -> void:
	state_transition.emit(self, state, generic_param)
