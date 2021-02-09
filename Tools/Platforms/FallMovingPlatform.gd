class_name FallMovingPlatform
extends MovingPlatform

func _ready():
	GameManager.PlatformManager.RegisterFallingPlatform(self)
