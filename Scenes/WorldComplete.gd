extends Area2D


export(String, FILE, "*.tscn") var next_scene

func _ready():
	GameManager.AreaManager.ConfigureSceneChange(self, next_scene)
