class_name MovingPlatform
extends KinematicBody2D

export var mov_enabled = true setget _set_enabled

func _ready():
	GameManager.PlatformManager.RegisterMovingPlatform(self)
		
func _set_enabled(v):
	mov_enabled = v
	if mov_enabled:
		GameManager.PlatformManager.RegisterMovingPlatform(self)
	else:
		GameManager.PlatformManager.stop_moving_platform(self)
