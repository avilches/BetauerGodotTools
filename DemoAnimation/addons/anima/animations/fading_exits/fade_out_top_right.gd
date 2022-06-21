func generate_animation(anima_tween: AnimaTween, data: Dictionary) -> void:
	var opacity_frames = [
		{ from = 1, to = 0 },
	]

	var size = AnimaNodesProperties.get_size(data.node)

	var position_frames = [
		{ percentage = 0, from = Vector2.ZERO },
		{ percentage = 100, to = Vector2(size.x, -size.y) },
	]

	anima_tween.add_relative_frames(data, "position", position_frames)
	anima_tween.add_frames(data, "opacity", opacity_frames)
