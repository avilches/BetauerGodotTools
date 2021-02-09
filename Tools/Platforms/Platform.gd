class_name Platform
extends KinematicBody2D

func _ready():
	GameManager.PlatformManager.RegisterPlatform(self)
