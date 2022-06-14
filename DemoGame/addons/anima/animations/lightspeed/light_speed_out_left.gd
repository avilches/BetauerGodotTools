func generate_animation(anima_tween: AnimaTween, data: Dictionary) -> void:
	var opacity_frames = [
		{ percentage = 0, from = 1 },
		{ percentage = 100, to = 0 },
	]

	var size = AnimaNodesProperties.get_size(data.node)
	var x_frames = [
		{ percentage = 0, from = 0 },
		{ percentage = 100, to = -size.x },
	]

	var skew_frames = [
		{ percentage = 0, from = 0 },
		{ percentage = 100, to = -1, easing = Anima.EASING.EASE_OUT_CIRC },
	]

	if data.node is Node2D:
		anima_tween.add_frames(data, "opacity", opacity_frames)
		anima_tween.add_relative_frames(data, "position:x", x_frames)
		anima_tween.add_frames(data, "skew:x", skew_frames)
