class_name Tower
extends Building

func _ready():
	is_pre_spawned = false
	building = self
	placed = false
	region.bake_finished.connect(free_to_rebake)
	self_index = director.tower_count
