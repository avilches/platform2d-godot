func generate_animation(anima_tween: AnimaTween, data: Dictionary) -> void:
	var opacity_frames = [
		{ from = 0, to = 1 },
	]

	anima_tween.add_frames(data, "opacity", opacity_frames)
