extends Area2D

func _ready():
	PlatformManager.add_area2d_platform_exit(self)
