extends Filler2D

func _ready():
	GameManager.AreaManager.RegisterDeathZone(self)

