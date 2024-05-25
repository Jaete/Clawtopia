class_name TowerPreview
extends Control

var tower_type: String
var tower_sprite: Sprite2D
var tower_texture: Texture2D

func _ready():
	tower_sprite = Sprite2D.new()
	tower_texture = load("res://Assets/Towers/"+ tower_type +".png")
	tower_sprite.texture = tower_texture
	if tower_type == "placeholder":
		tower_sprite.apply_scale(Vector2(0.2, 0.2))
	add_child(tower_sprite)
	position_sprite.call_deferred()
	pass

func position_sprite():
	tower_sprite.global_position = self.global_position

