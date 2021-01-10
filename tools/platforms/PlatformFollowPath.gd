class_name PlatformFollowPath
extends MovingPlatform

onready var path = $'Platform path/PathFollow2D'
onready var original = position
export var move_speed:float = 10

func _enter_tree():
	original = position
		
func _physics_process(delta):
	if (mov_enabled):
		position = original + path.position
		path.set_offset(path.get_offset() + move_speed * delta)
