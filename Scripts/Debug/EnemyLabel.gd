extends Label

@onready var state_machine: EnemyStateMachine = $"../StateMachine"

func _physics_process(delta):
	set_text(str("State: ", state_machine.current_state.name))
