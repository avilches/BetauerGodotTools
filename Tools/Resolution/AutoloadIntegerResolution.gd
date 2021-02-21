extends Node
# https://github.com/Yukitty/godot-addon-integer_resolution_handler

# IntegerResolutionHandler autoload.
# Watches for window size changes and handles
# game screen scaling with exact integer
# multiples of a base resolution in mind.

const SETTING_BASE_WIDTH = "display/window/integer_resolution_handler/base_width"
const SETTING_BASE_HEIGHT = "display/window/integer_resolution_handler/base_height"

#var base_resolution := Vector2(320, 180)  # 1920x1080 / 6
var original_resolution := Vector2(480, 270) # 1920x1080 / 4
var scale = 1
#var base_resolution := Vector2(960, 540)   # 1920x1080 / 2
#var base_resolution := Vector2(1920, 1080)
#var base_resolution := Vector2(2560, 1440)  # 1920x1080 / 4
var stretch_mode: int
var stretch_aspect: int
onready var stretch_shrink: float = 1  # ProjectSettings.get_setting("display/window/stretch/shrink")
onready var _root: Viewport = get_node("/root")
var base_resolution = original_resolution

#disabled: while the framebuffer will be resized to match the game window, nothing will be upscaled or downscaled (this includes GUIs).
#2d: the framebuffer is still resized, but GUIs can be upscaled or downscaled. This can result in blurry or pixelated fonts.
#viewport: the framebuffer is resized, but computed at the original size of the project. The whole rendering will be pixelated. You generally do not want this, unless it's part of the game style.





var DEBUG_INFO = true
var ALLOW_CHANGE_STRETCH = true
func _ready():
	load_project_settings()
	stretch_aspect = SceneTree.STRETCH_ASPECT_KEEP
	configure()
	# warning-ignore:return_value_discarded
	get_tree().connect("screen_resized", self, "update_resolution")

func _unhandled_input(event):
	if event is InputEventKey && event.is_pressed():

		if ALLOW_CHANGE_STRETCH:
            if event.scancode == KEY_Q:
                stretch_mode = SceneTree.STRETCH_MODE_2D
            if event.scancode == KEY_W:
                stretch_mode = SceneTree.STRETCH_MODE_VIEWPORT
            if event.scancode == KEY_E:
                stretch_mode = SceneTree.STRETCH_MODE_DISABLED
			
		if event.scancode == KEY_ENTER:
			OS.window_fullscreen = !OS.window_fullscreen
			
		if event.scancode == KEY_A:
			stretch_aspect = SceneTree.STRETCH_ASPECT_KEEP
		if event.scancode == KEY_S:
			stretch_aspect = SceneTree.STRETCH_ASPECT_KEEP_HEIGHT
		if event.scancode == KEY_D:
			stretch_aspect = SceneTree.STRETCH_ASPECT_KEEP_WIDTH
		if event.scancode == KEY_F:
			stretch_aspect = SceneTree.STRETCH_ASPECT_EXPAND
		if event.scancode == KEY_G:
			stretch_aspect = SceneTree.STRETCH_ASPECT_IGNORE

		print(base_resolution, " ",stretch_mode," ",stretch_aspect)
		
		if event.scancode == KEY_1:
			base_resolution = original_resolution * 1
		if event.scancode == KEY_2:
			base_resolution = original_resolution * 1.1
		if event.scancode == KEY_3:
			base_resolution = original_resolution * 1.2
		if event.scancode == KEY_4:
			base_resolution = original_resolution * 1.3
		if event.scancode == KEY_5:
			base_resolution = original_resolution * 1.4
		if event.scancode == KEY_6:
			base_resolution = original_resolution * 1
		if event.scancode == KEY_7:
			base_resolution = original_resolution * 1.1
		if event.scancode == KEY_8:
			base_resolution = original_resolution * 1.2
		if event.scancode == KEY_9:
			base_resolution = original_resolution * 1.3
		if event.scancode == KEY_5:
			base_resolution = original_resolution * 1.4


		update_resolution()
		

func load_project_settings():
	# Parse project settings
	if ProjectSettings.has_setting(SETTING_BASE_WIDTH):
		base_resolution.x = ProjectSettings.get_setting(SETTING_BASE_WIDTH)
	if ProjectSettings.has_setting(SETTING_BASE_HEIGHT):
		base_resolution.y = ProjectSettings.get_setting(SETTING_BASE_HEIGHT)

	match ProjectSettings.get_setting("display/window/stretch/mode"):
		"2d":
			stretch_mode = SceneTree.STRETCH_MODE_2D
		"viewport":
			stretch_mode = SceneTree.STRETCH_MODE_VIEWPORT
		_:
			stretch_mode = SceneTree.STRETCH_MODE_DISABLED

	match ProjectSettings.get_setting("display/window/stretch/aspect"):
		"keep":
			stretch_aspect = SceneTree.STRETCH_ASPECT_KEEP
		"keep_height":
			stretch_aspect = SceneTree.STRETCH_ASPECT_KEEP_HEIGHT
		"keep_width":
			stretch_aspect = SceneTree.STRETCH_ASPECT_KEEP_WIDTH
		"expand":
			stretch_aspect = SceneTree.STRETCH_ASPECT_EXPAND
		_:
			stretch_aspect = SceneTree.STRETCH_ASPECT_IGNORE

func configure():
	# Enforce minimum resolution.
	OS.min_window_size = base_resolution

	# Remove default stretch behavior.
	var tree: SceneTree = get_tree()
	tree.set_screen_stretch(SceneTree.STRETCH_MODE_DISABLED, SceneTree.STRETCH_ASPECT_IGNORE, base_resolution, 1)

	# Start tracking resolution changes and scaling the screen.
	update_resolution()



func update_resolution():
	var video_mode: Vector2 = OS.window_size
	if OS.window_fullscreen:
		video_mode = OS.get_screen_size()

	var scale := int(max(floor(min(video_mode.x / base_resolution.x, video_mode.y / base_resolution.y)), 1))
	var screen_size: Vector2 = base_resolution
	var viewport_size: Vector2 = screen_size * scale
	var overscan: Vector2 = ((video_mode - viewport_size) / scale).floor()
	var margin: Vector2
	var margin2: Vector2

	match stretch_aspect:
		SceneTree.STRETCH_ASPECT_KEEP_WIDTH:
			screen_size.y += overscan.y
		SceneTree.STRETCH_ASPECT_KEEP_HEIGHT:
			screen_size.x += overscan.x
		SceneTree.STRETCH_ASPECT_EXPAND, SceneTree.STRETCH_ASPECT_IGNORE:
			screen_size += overscan
	viewport_size = screen_size * scale
	margin = (video_mode - viewport_size) / 2
	margin2 = margin.ceil()
	margin = margin.floor()


	match stretch_mode:
		SceneTree.STRETCH_MODE_VIEWPORT:
			_root.set_size((screen_size / stretch_shrink).floor())
			_root.set_attach_to_screen_rect(Rect2(margin, viewport_size))
			_root.set_size_override_stretch(false)
			_root.set_size_override(false)
			if DEBUG_INFO:
				print("(Viewport Mode) Base resolution:",str(base_resolution.x),"x", str(base_resolution.y),\
				" Video resolution:",str(video_mode.x),"x",str(video_mode.y), \
				" Size:", (screen_size / stretch_shrink).floor(), "(Screen size ", screen_size,"/",stretch_shrink," stretch shrink)",\
				" Viewport rect: ", margin, " ",viewport_size)
		SceneTree.STRETCH_MODE_2D, _:
			_root.set_size((viewport_size / stretch_shrink).floor())
			_root.set_attach_to_screen_rect(Rect2(margin, viewport_size))
			_root.set_size_override_stretch(true)
			_root.set_size_override(true, (screen_size / stretch_shrink).floor())
			if DEBUG_INFO:
				print("(2D model) Base resolution:",str(base_resolution.x),"x", str(base_resolution.y),\
				" Video resolution:",str(video_mode.x),"x",str(video_mode.y), \
				" Size:", (viewport_size / stretch_shrink).floor(), " (Viewport size ", viewport_size,"/",stretch_shrink," stretch shrink)",\
				" Viewport rect: ", margin, " ",viewport_size,
				" Size override:", (screen_size / stretch_shrink).floor(), "(Screen size ", screen_size,"/",stretch_shrink," stretch shrink)")

	VisualServer.black_bars_set_margins(max(0, int(margin.x)), max(0, int(margin.y)), max(0, int(margin2.x)), max(0, int(margin2.y)))
