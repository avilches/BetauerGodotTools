class_name FallPlatform
extends KinematicBody2D

func _ready():
	PlatformManager.RegisterFallingPlatform(self)
