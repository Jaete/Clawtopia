class_name TowerShot
extends CharacterBody2D

@onready var nav: NavigationAgent2D = $NavigationAgent2D
@onready var tower: Tower = get_parent()
var current_target: int
# Called when the node enters the scene tree for the first time.
func _ready():
	nav.target_position = tower.targets[current_target]
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
