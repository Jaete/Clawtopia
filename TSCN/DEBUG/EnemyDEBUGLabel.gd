extends RichTextLabel

@onready var enemy: Enemy = $".."
@onready var fsm: EnemyStateMachine = $"../FSM"

func _process(delta):
	set_text(str("State: ", fsm.current_state.name,
	"\nSpeed: ", enemy.speed,
	"\nAccel: ", enemy.accel))
