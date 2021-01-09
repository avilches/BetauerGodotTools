tool
class_name Filler2D
extends CollisionShape2D

export (Color) var color = Color(1, 1, 1, 1) setget set_color
export var enabled_editor = false setget set_enabled_editor
export var enabled_game = false setget set_enabled_game

func set_color(new_color):
	color = new_color
	update()
	
func set_enabled_editor(v):
	enabled_editor = v
	update()
	
func set_enabled_game(v):
	enabled_game = v
	update()

func _draw():
	if Engine.editor_hint:
		if !enabled_editor: return
	else:
		if !enabled_game: return
		
	var offset_position = Vector2(0, 0)

	if shape is CircleShape2D:
		draw_circle(offset_position, shape.radius, color)
	elif shape is RectangleShape2D:
		var rect = Rect2(offset_position - shape.extents, shape.extents * 2.0)
		draw_rect(rect, color)
	elif shape is CapsuleShape2D:
		var upper_circle_position = offset_position + Vector2(0, shape.height * 0.5)
		draw_circle(upper_circle_position, shape.radius, color)

		var lower_circle_position = offset_position - Vector2(0, shape.height * 0.5)
		draw_circle(lower_circle_position, shape.radius, color)

		var rect_position = offset_position - Vector2(shape.radius, shape.height * 0.5)
		var rect = Rect2(rect_position, Vector2(shape.radius * 2, shape.height))
		draw_rect(rect, color)
