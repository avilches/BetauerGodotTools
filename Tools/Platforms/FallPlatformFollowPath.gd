class_name FallPlatformFollowPath
extends PlatformFollowPath

func _ready():
	GameManager.PlatformManager.RegisterFallingPlatform(self)
