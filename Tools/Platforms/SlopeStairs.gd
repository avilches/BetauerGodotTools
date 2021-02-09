extends StaticBody2D

func _ready():
	GameManager.PlatformManager.RegisterSlopeStairs(self)
	
#func _process(delta):
#	$Label.text = str(GameManager.PlatformManager.IsSlopeStairs(self))

