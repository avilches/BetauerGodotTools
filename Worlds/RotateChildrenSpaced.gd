extends Node2D

export var radius = Vector2(50, 50)
export var rotation_duration = 4.0

var speed = TAU / rotation_duration # speed is radians/seconds. TAU is a whole round in one second
var angle = 0
var platforms
var spacing

func _physics_process(delta):
	angle = wrapf(angle + speed * delta, 0, TAU) # Infinite rotation (in radians)
	_update_platforms()
	
func _update_platforms():
	if platforms.size () == 0: return
	for i in platforms.size():
		var new_x = sin(spacing * i + angle) * radius.x
		var new_y = cos(spacing * i + angle) * radius.y
		platforms[i].position = Vector2(new_x, new_y)
		
func find_platforms():
	platforms = []
	for child in get_children(): platforms.append(child)
	if platforms.size() > 0:
		spacing = TAU / platforms.size() # spacing between platforms (in radians)

func _ready():
	find_platforms()
