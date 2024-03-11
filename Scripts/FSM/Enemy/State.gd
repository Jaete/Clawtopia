class_name EnemyState
extends Node2D

signal state_transition()

@onready var state_machine: EnemyStateMachine = get_parent()
@onready var enemy: Enemy = $"../.."

func enter() -> void:
	pass

func exit() -> void:
	pass

func update(_delta: float) -> void:
	pass

func change_state(state: String) -> void:
	state_transition.emit(self, state)

func logger_divider(string: String):
	print("\n#====== " + string + "======#\n")
