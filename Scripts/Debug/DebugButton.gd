extends Button

signal release_the_kitties()

var cat_count: int = 0
var times_clicked: = 0
#@onready var tower: Tower = $"../Tower"
#@onready var tower_detection: CollisionShape2D = tower.get_node("EnemyDetection/DetectionRange")
@onready var main = get_node("/root/SceneManager/Director")
@onready var cat: PackedScene = preload("res://TSCN/Objects/enemy.tscn")
@onready var spawn_point: Node2D = get_node("/root/SceneManager/Director/SpawnPoint")

var ultra_spawn: bool

func _ready():
	release_the_kitties.connect(release)

func _on_pressed():
	match(name):
		"Spawn_Cat":
				release_the_kitties.emit()
			
		#"Change_Tower_Attack":
			#if tower.attack_type == tower.ARTISTS:
				#tower.attack_type = tower.FIGHTERS
				#var attack_range: CircleShape2D = CircleShape2D.new()
				#attack_range.set_radius(50)
				#tower_detection.set_shape(attack_range)
			#else:
				#tower.attack_type = tower.ARTISTS
				#var attack_range: CircleShape2D = CircleShape2D.new()
				#attack_range.set_radius(120)
				#tower_detection.set_shape(attack_range)
			#pass

func release():
	times_clicked += 1
	ultra_spawn = true

func _physics_process(delta):
	if ultra_spawn:
		cat_count += 1
		var new_cat: Enemy = cat.instantiate()
		new_cat.set_name(str("CAT ", cat_count))
		main.add_child(new_cat)
		new_cat.global_position = spawn_point.global_position
		if cat_count == 80 * times_clicked:
			ultra_spawn = false
			
	pass
