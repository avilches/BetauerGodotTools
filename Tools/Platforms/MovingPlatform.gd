class_name MovingPlatform
extends KinematicBody2D

export var mov_enabled = true setget _set_enabled

func _ready():
	PlatformManager.register_moving_platform(self)
		
func _set_enabled(v):
	mov_enabled = v
	if mov_enabled:
		PlatformManager.register_moving_platform(self)
	else:
		PlatformManager.stop_moving_platform(self)
