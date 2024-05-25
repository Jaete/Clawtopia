class_name FighterStateMachine
extends Node2D

var states: Dictionary = {}
var current_state: AllyState

func _ready() -> void:
	logger_divider("STATE MACHINE CREATION (FIGHTER CLASS)")
	var state_list: Array[Node] = get_children()
	for state in state_list:
		if state.name == "State":
			pass
		else:
			states[state.name] = state
			states[state.name].state_transition.connect(change_state)
	print("State List Ready: ", states.values())
	current_state = $Idle
	logger_divider("STATE MACHINE READY (FIGHTER CLASS)")

func _physics_process(delta) -> void:
	if current_state.name != "Idle":
		current_state.update(delta)

func change_state(source_state: AllyState, next_state: String) -> void:
	logger_divider("CHANGING STATE (FIGHTER CLASS)")
	if !next_state in states.keys():
		print("Next state doesn't exist. Aborting.")
		return
	if source_state.name != current_state.name:
		print("Repeated change state call. Aborting.")
		return
	var next: AllyState = states[next_state]
	print("Exiting current state: ", current_state)
	current_state.exit()
	print("Entering next state: ", next)
	next.enter()
	current_state = next
	print("Current state: ", current_state)
	logger_divider("STATE CHANGED (FIGHTER CLASS)")

func logger_divider(string: String) -> void:
	print("\n#====== " + string + " ======#\n")
