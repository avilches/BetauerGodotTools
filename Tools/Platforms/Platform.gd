class_name Platform
extends KinematicBody2D

func _ready():
	PlatformManager.register_platform(self)
