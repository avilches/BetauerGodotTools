extends Area2D

func _ready():
	PlatformManager.AddArea2DSlopeStairsEnabler(self)
