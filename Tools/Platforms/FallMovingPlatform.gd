class_name FallMovingPlatform
extends MovingPlatform

func _ready():
	PlatformManager.RegisterFallingPlatform(self)
