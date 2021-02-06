class_name Platform
extends KinematicBody2D

func _ready():
	PlatformManager.RegisterPlatform(self)
