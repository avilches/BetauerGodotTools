extends Area2D


func _ready():
	self.connect("body_entered", self, "on_enter")
	
func on_enter(body):
	if Global.IsPlayer(body):
		$"../MultipleAligned".Start()
