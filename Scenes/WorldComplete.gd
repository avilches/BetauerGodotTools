extends Area2D

func _physics_process(delta):
	var bodies = get_overlapping_bodies();
	for body in bodies:
		if body.name == "Player":
			print(body.name)
			get_tree().change_scene("Scenes/World2.tscn")
			
