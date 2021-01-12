class_name FallPlatform
extends KinematicBody2D

func _ready():
	PlatformManager.register_falling_platform(self)
