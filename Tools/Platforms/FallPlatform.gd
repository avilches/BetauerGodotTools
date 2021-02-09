class_name FallPlatform
extends KinematicBody2D

func _ready():
	GameManager.PlatformManager.RegisterFallingPlatform(self)
