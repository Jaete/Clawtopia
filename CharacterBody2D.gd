extends CharacterBody2D

var speed = 300
var accel = 7

@onready var nav: NavigationAgent2D = $NavigationAgent2D

func _physics_process(delta):
	var direction = Vector3()
	nav.target_position = get_global_mouse_position()
	direction = (nav.get_next_path_position() - global_position).normalized()
	velocity = velocity.lerp(direction * speed, accel * delta)
	look_at(nav.target_position)
	move_and_slide()
