class_name StageArea
extends Filler2D

func _ready():
	GameManager.AreaManager.RegisterStage(self)
