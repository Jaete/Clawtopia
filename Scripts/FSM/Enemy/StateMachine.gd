class_name EnemyStateMachine
extends Node

var states: Dictionary = {}
var initial_state: EnemyState
var current_state: EnemyState

var changing_state: bool = false

func _ready() -> void:
	var state_list: Array[Node] = get_children()
	for state in state_list:
		if state.name == "State":
			pass
		else:
			states[state.name] = state
			states[state.name].state_transition.connect(change_state)
	current_state = $Idle

func _physics_process(delta) -> void:
	if !changing_state:
		current_state.update(delta)

func change_state(source_state: EnemyState, next_state: String, generic_param: Variant = null) -> void:
	changing_state = true
	if !next_state in states.keys():
		return
	if source_state.name != current_state.name:
		return
	var next: EnemyState = states[next_state]
	current_state.exit()
	next.enter(generic_param)
	current_state = next
	changing_state = false
