extends StaticBody2D

func _ready():
	PlatformManager.register_slope_stairs(self)
	
#func _process(delta):
#	$Label.text = str(PlatformManager.is_a_slope_stairs(self))

