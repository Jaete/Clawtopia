extends Label

@onready var fighter_fsm: FighterStateMachine = $"../FSM"

func _physics_process(delta):
	text = str("State: ", fighter_fsm.current_state.name)
