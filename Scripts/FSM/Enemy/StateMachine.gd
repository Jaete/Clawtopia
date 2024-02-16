class_name EnemyStateMachine
extends Node2D

var states: Dictionary = {}
@export var initial_state: EnemyState
var current_state: EnemyState

func _ready() -> void:
	logger_divider("STATE MACHINE CREATION")
	var state_list: Array[Node] = get_children()
	for state in state_list:
		if state.name == "State":
			pass
		else:
			states[state.name] = state
	print("State List Ready: ", states.values())
	current_state = initial_state
	current_state.state_transition.connect(change_state)
	logger_divider("STATE MACHINE READY")

func _physics_process(delta) -> void:
	if current_state.name != "Idle":
		current_state.update(delta)

func change_state(source_state: EnemyState, next_state: String) -> void:
	logger_divider("CHANGING STATE")
	if !next_state in states.keys():
		print("Next state doesn't exist. Aborting.")
		return
	if source_state.name != current_state.name:
		print("Repeated change state call. Aborting.")
		return
	var next: EnemyState = states[next_state]
	print("Exiting current state: ", current_state)
	current_state.exit()
	print("Entering next state: ", next)
	next.enter()
	current_state = next
	print("Current state: ", current_state)
	logger_divider("STATE CHANGED")

func logger_divider(string: String) -> void:
	print("\n#====== " + string + " ======#\n")
