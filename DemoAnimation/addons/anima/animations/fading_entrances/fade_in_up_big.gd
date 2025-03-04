func generate_animation(anima_tween: AnimaTween, data: Dictionary) -> void:
	var opacity_frames = [
		{ from = 0, to = 1 },
	]

	var size = AnimaNodesProperties.get_size(data.node)

	var position_frames = [
		{ percentage = 0, from = 1000 },
		{ percentage = 100, to = -1000 },
	]

	anima_tween.add_relative_frames(data, "y", position_frames)
	anima_tween.add_frames(data, "opacity", opacity_frames)
