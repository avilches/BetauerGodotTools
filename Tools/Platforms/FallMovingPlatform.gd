class_name FallMovingPlatform
extends MovingPlatform

func _ready():
	PlatformManager.register_falling_platform(self)
