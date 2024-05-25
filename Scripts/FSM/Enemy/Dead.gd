class_name EnemyDead
extends EnemyState

func enter() -> void:
	enemy.get_parent().remove_child(enemy)
	enemy.call_deferred("queue_free")
	pass

func exit() -> void:
	pass

func update(_delta: float) -> void:
	pass
