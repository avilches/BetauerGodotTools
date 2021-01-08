class_name DeathArea
extends Filler2D

func _ready():
	var area2D = get_parent()
	if area2D is Area2D:
		area2D.connect("body_entered", self, "on_body_entered")
	else:
		var message = "Error in DeathArea: parent should be an Area2D"
		print(message)
		push_error(message)

func on_body_entered(who):
	if GameManager.isPlayer(who):
		GameManager.emit_death("DeathArea")
