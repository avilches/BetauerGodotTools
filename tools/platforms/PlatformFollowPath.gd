class_name PlatformFollowPath
extends MovingPlatform

onready var path = $'Platform path/PathFollow2D'

onready var original = position

func _ready():
	original = position

func _enter_tree():
	original = position
		
var move_speed = 10
func _physics_process(delta):
	if (mov_enabled):
		position = original + path.position
		path.set_offset(path.get_offset() + 10 * delta)
