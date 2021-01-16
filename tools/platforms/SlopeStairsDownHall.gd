extends Area2D

func _ready():
	PlatformManager.add_area2d_slope_stairs_down(self)
