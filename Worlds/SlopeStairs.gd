extends StaticBody2D

func _ready():
	PlatformManager.register_slope_stairs(self)

