class_name EnemyState
extends Node2D

signal state_transition()

@onready var state_machine: EnemyStateMachine = get_parent()
@onready var tower_shot: PackedScene = preload("res://TSCN/Objects/tower_shot.tscn")
@onready var enemy: Enemy = $"../.."
@onready var nav: NavigationAgent2D = $"../../NavigationAgent2D"

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
