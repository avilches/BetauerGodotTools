class_name MovingPlatform
extends KinematicBody2D

export var mov_enabled = true setget _set_enabled

func _ready():
	PlatformManager.RegisterMovingPlatform(self)
		
func _set_enabled(v):
	mov_enabled = v
	if mov_enabled:
		PlatformManager.RegisterMovingPlatform(self)
	else:
		PlatformManager.stop_moving_platform(self)
