extends Node

# don't forget to use stretch mode 'viewport' and aspect 'ignore'
onready var viewport = get_viewport()

func _ready():
	get_tree().connect("screen_resized", self, "_screen_resized")

func _screen_resized():
	var window_size = OS.get_window_size()

	# see how big the window is compared to the viewport size
	# floor it so we only get round numbers (0, 1, 2, 3 ...)
	var scale_x = floor(window_size.x / viewport.size.x)
	var scale_y = floor(window_size.y / viewport.size.y)

	# use the smaller scale with 1x minimum scale
	var scale = max(1, min(scale_x, scale_y))

	# extend the viewport to actually fit the screen as much as possible, in pixel perfect amounts
	#find the scaled size difference (basically visual pixel difference) between the screen and viewport dimensions
	var sizeDiff = window_size - (viewport.size * scale)
	var pixelDiff = (sizeDiff/scale).ceil()
	#If either dimension is odd, make it even by adding a pixel (otherwise you might have everything offset by a half pixel)
	if int(pixelDiff.x) % 2 == 1:
		pixelDiff.x += 1
	if int(pixelDiff.y) % 2 == 1:
		pixelDiff.y += 1
	#Now actually scale up the viewport to make it fill the screen
	viewport.set_size(viewport.size + pixelDiff)


	# find the coordinate we will use to center the viewport inside the window
	var diff = window_size - (viewport.size * scale)
	var diffhalf = (diff * 0.5).floor()

	# attach the viewport to the rect we calculated
	viewport.set_attach_to_screen_rect(Rect2(diffhalf, viewport.size * scale))
