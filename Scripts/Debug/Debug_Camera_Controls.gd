class_name UIDirector
extends CanvasLayer

@onready var camera: Camera2D = $"../Camera2D"
@onready var level: Node2D = $".."

const NORMAL: Vector2 = Vector2(1.0,1.0)
const ZOOMED_IN: Vector2 = Vector2(1.5,1.5)
const ZOOMED_OUT: Vector2 = Vector2(0.6,0.6)

func _ready():
	camera.zoom = NORMAL
	pass

func _on_move_camera_left_pressed():
	camera.global_position.x = camera.global_position.x - 50
	pass # Replace with function body.


func _on_move_camera_right_pressed():
	camera.global_position.x = camera.global_position.x + 50
	pass # Replace with function body.


func _on_move_camera_up_pressed():
	camera.global_position.y = camera.global_position.y - 50
	pass # Replace with function body.


func _on_move_camera_down_pressed():
	camera.global_position.y = camera.global_position.y + 50
	pass # Replace with function body.


func _on_zoom_in_pressed():
	if camera.zoom.x == NORMAL.x:
		camera.set_zoom(ZOOMED_IN)
	if camera.zoom.x == ZOOMED_OUT.x:
		camera.set_zoom(NORMAL)
	print(str("CURRENT ZOOM: ", camera.zoom))


func _on_zoom_out_pressed():
	if(camera.zoom == NORMAL):
		camera.set_zoom(ZOOMED_OUT)
	if(camera.zoom == ZOOMED_IN):
		camera.set_zoom(NORMAL)
	print(str("CURRENT ZOOM: ", camera.zoom))
