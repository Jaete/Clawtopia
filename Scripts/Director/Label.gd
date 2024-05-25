extends Label

@onready var catcount := $"../ControlButtons/Spawn_Cat"
var cat_count_label: int

func _process(delta):
	cat_count_label = catcount.cat_count
	set_text(str("Cats: ", cat_count_label))
	
