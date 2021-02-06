class_name FallPlatformFollowPath
extends PlatformFollowPath

func _ready():
	PlatformManager.RegisterFallingPlatform(self)
