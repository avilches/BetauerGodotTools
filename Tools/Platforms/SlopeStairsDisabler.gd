extends Area2D

func _ready():
	PlatformManager.AddArea2DSlopeStairsDisabler(self)
