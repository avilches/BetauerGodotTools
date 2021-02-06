extends StaticBody2D

func _ready():
	PlatformManager.RegisterSlopeStairsCover(self)
	
#func _process(delta):
#	$Label.text = str(PlatformManager.IsSlopeStairs(self))

