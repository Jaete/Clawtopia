class_name EnemyMove
extends EnemyState

@onready var path: PathFollow2D = $"../../.."

func enter():
	pass

func exit():
	pass

func update(_delta: float) -> void:
	path.set_progress(path.progress + 1)
	pass


