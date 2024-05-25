extends CharacterBody2D

var movement_speed: float = 25.0
var movement_target_position: Vector2 = Vector2(452.0,173.0)

@onready var navigation_agent: NavigationAgent2D = $NavigationAgent2D

func _ready():
	# These values need to be adjusted for the actor's speed
	# and the navigation layout.
	#navigation_agent.path_desired_distance = 4.0
	#navigation_agent.target_desired_distance = 4.0
	# Make sure to not await during _ready.
	#call_deferred("actor_setup")
	pass

func _physics_process(delta):
	pass
	#if navigation_agent.is_navigation_finished():
		#return
	#set_movement_target(get_global_mouse_position())
	#var current_agent_position: Vector2 = global_position
	#var next_path_position: Vector2 = navigation_agent.get_next_path_position()
	#var new_velocity: Vector2 = current_agent_position.direction_to(next_path_position) * movement_speed
	#var going_left: bool =  new_velocity.x < 0
	#var going_up: bool = new_velocity.y < 0
	#if  going_left && going_up:
		#print("INDO PARA CIMA-ESQUERDA")
	#if !going_left && going_up:
		#print("INDO PARA CIMA-DIREITA")
	#if going_left && !going_up:
		#print("INDO PARA BAIXO-ESQUERDA")
	#if !going_left && !going_up:
		#print("INDO PARA BAIXO-DIREITA")
	#navigation_agent.set_velocity(new_velocity)
	#move_and_slide()

func actor_setup():
	# Wait for the first physics frame so the NavigationServer can sync.
	await get_tree().physics_frame

	# Now that the navigation map is no longer empty, set the movement target.
	var tilemap: TileMap = $"../Estradas/TileMap"
	navigation_agent.set_navigation_map(tilemap.get_layer_navigation_map(2))


func set_movement_target(movement_target: Vector2):
	navigation_agent.set_target_position(movement_target)

func _on_navigation_agent_2d_velocity_computed(safe_velocity):
	#velocity = safe_velocity
	#move_and_slide()
	pass # Replace with function body.
