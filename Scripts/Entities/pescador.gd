class_name Enemy
extends CharacterBody2D

var speed: float
var hp: int
var damage: int
var cat_type: String

var nearby_towers: Array[Node]

# Called when the node enters the scene tree for the first time.
func _ready():
	match cat_type:
		1:
			speed = 1
			hp = 5
			damage = 2
		2:
			speed = 1
			hp = 3
			damage = 4
		3:
			speed = 2
			hp = 4
			damage = 3
	var skin_variation = randi_range(1,3)
	match(skin_variation):
		1:
			var texture: Texture2D = load("res://Assets/Enemies/gatinho1.png")
			$Sprite.set_texture(texture)
		2:  
			var texture: Texture2D = load("res://Assets/Enemies/gatinho2.png")
			$Sprite.set_texture(texture)
		3: 
			var texture: Texture2D = load("res://Assets/Enemies/gatinho3.png")
			$Sprite.set_texture(texture)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
