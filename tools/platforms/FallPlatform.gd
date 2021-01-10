class_name FallPlatform
extends Platform

func _ready():
	PlatformManager.register_falling_platform(self)
