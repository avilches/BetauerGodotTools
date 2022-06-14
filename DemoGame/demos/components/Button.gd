extends Button

export (String) var scene = ''

func _on_ButtonAnimations_pressed():
	get_tree().change_scene(scene)
