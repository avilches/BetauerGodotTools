extends StaticBody2D

func _ready():
	PlatformManager.register_slope_stairs_cover(self)
	
#func _process(delta):
#	$Label.text = str(PlatformManager.IsSlopeStairs(self))

