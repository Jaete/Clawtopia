class_name GreatCommune
extends Building

func _ready():
	is_pre_spawned = true
	building = self
	placed = false
	region.bake_finished.connect(free_to_rebake)
	self_index = director.tower_count
	rebake_add_building()
