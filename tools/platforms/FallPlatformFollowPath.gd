class_name FallPlatformFollowPath
extends PlatformFollowPath

func _ready():
	PlatformManager.register_falling_platform(self)
