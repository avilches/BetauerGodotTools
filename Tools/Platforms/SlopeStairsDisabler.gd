extends Area2D

func _ready():
	GameManager.PlatformManager.AddArea2DSlopeStairsDisabler(self)
