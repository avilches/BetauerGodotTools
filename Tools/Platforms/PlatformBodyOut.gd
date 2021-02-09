extends Area2D

func _ready():
	GameManager.PlatformManager.AddArea2DFallingPlatformExit(self)
