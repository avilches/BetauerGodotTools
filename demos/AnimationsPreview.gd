extends Control

onready var durationEdit = $HBoxContainer/ScrollContainer/PanelContainer/ListContainer/HBoxContainer/VBoxContainer/HBoxContainer/DurationEdit

func _ready():
	_setup_list()


func _process(delta):
#	if animating:
#		print(myNodeAnimating," ",myNodeAnimating.rect_scale)
	pass

var animating = false

func _setup_list() -> void:
	var animations = Anima.get_available_animations()
	var base = Anima.BASE_PATH
	var old_category := ''

	for item in animations:
		var category_and_file = item.replace(base, '').split('/')
		var category = category_and_file[0]
		var file_and_extension = category_and_file[1].split('.')
		var file = file_and_extension[0]

		if category != old_category:
			var header = create_new_header(category)
			$HBoxContainer/ScrollContainer/PanelContainer/ListContainer.add_child(header)

		var button := Button.new()
		button.set_text(file.replace('_', ' ').capitalize())
		button.set_text_align(Button.ALIGN_LEFT)
		button.set_meta('script', file)
		button.connect("pressed", self, '_on_animation_button_pressed', [button])

		$HBoxContainer/ScrollContainer/PanelContainer/ListContainer.add_child(button)
		old_category = category

func create_new_header(text: String) -> PanelContainer:
	var container := PanelContainer.new()
	var label := Label.new()

	label.set_text(text.replace('_', ' ').capitalize())
	container.add_child(label)
	
	var style := StyleBoxFlat.new()
	style.bg_color = Color('#404553')
	style.content_margin_top = 12
	style.content_margin_left = 8
	style.content_margin_bottom = 12
	style.content_margin_right = 8

	container.add_stylebox_override('panel', style)

	return container

func _on_animation_button_pressed(button: Button) -> void:
	var script_name: String = button.get_meta('script')

	var duration = float(durationEdit.text)

	_play_animation($HBoxContainer/VBoxContainer/SpriteContainer/Control2/SpriteTest, get_node("/root/Node2D/Node2D2/SpriteContainer/Control2/SpriteTest"), button)
	_play_animation($HBoxContainer/VBoxContainer/ControlContainer/ControlTest, get_node("/root/Node2D/Node2D2/ControlContainer/ControlTest"), button)

func _play_animation(animaNode: Node, node: Node, button: Button):
	var script_name: String = button.get_meta('script')
	var duration = float(durationEdit.text)

	var animaParent = animaNode.get_parent()
	var animaClone = animaNode.duplicate()
	_remove_duplicate(animaParent, animaNode)
	animaParent.add_child(animaClone)
	animaClone.show()
	animaNode.hide()

	var parent = node.get_parent()
	var clone = node.duplicate()
	_remove_duplicate(parent, node)
	parent.add_child(clone)
	clone.show()
	node.hide()

	var anima = Anima.begin(animaClone, 'control_test')
	anima.then({ node = animaClone, animation = script_name, duration = duration })
	anima.play()
	
	Global.Animate(clone, script_name, duration)
	animating = true
	myNodeAnimating = clone

	yield(anima, "animation_completed")
	animating = false
#
	print("End animation")
	_remove_duplicate(animaParent, animaNode)
	_remove_duplicate(parent, node)
#
	animaNode.show()
	node.show()

var myNodeAnimating

func _remove_duplicate(parent: Node, node_to_ignore: Node) -> void:
	for child in parent.get_children():
		if child != node_to_ignore:
			child.queue_free()

func _on_control_animation_completed(animation_player: AnimationPlayer) -> void:
	print(animation_player)

func _on_ControlCheckbox_pressed():
	$HBoxContainer/VBoxContainer/ControlLabel.visible = $HBoxContainer/ScrollContainer/PanelContainer/ListContainer/VBoxContainer/ControlCheckbox.pressed
	$HBoxContainer/VBoxContainer/ControlContainer.visible = $HBoxContainer/ScrollContainer/PanelContainer/ListContainer/VBoxContainer/ControlCheckbox.pressed

func _on_SpriteCheckbox_pressed():
	$HBoxContainer/VBoxContainer/SpriteLabel.visible = $HBoxContainer/ScrollContainer/PanelContainer/ListContainer/VBoxContainer/SpriteCheckbox.pressed
	$HBoxContainer/VBoxContainer/SpriteContainer.visible = $HBoxContainer/ScrollContainer/PanelContainer/ListContainer/VBoxContainer/SpriteCheckbox.pressed

func generate_animation(anima_tween: AnimaTween, data: Dictionary) -> void:
	anima_tween.add_animation_data(data)
	return


