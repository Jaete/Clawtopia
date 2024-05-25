extends Label

@onready var towerFSM: TowerStateMachine = $"../StateMachine"

func _physics_process(delta):
	text = str("State: ", towerFSM.current_state.name)
