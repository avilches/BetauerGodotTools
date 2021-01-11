extends Node2D

export var radius = 50
# export var rotation_duration = 4.0
# var speed = TAU / rotation_duration # speed is radians/seconds. TAU is a whole round in one second

var angle = 0
var platforms
var spacing

const CLOCK_THREE = PI/2
const CLOCK_NINE = -PI/2
const CLOCK_SIX = 0
const CLOCK_NOON = PI

func _ready():
	find_platforms()
	half_round_down()  # media vuelta por abajo
#	whole_round() # vuelta entera

func whole_round():
	var seq := TweenSequence.new(get_tree())
	seq.append(self, "angle", TAU, 4).from(0).set_trans(Tween.TRANS_LINEAR)
	seq.set_loops()

func half_round_down():

	var start = CLOCK_NINE
	var end = CLOCK_THREE
	var seq := TweenSequence.new(get_tree())
	seq.append(self, "angle", end, 2).from(start).set_trans(Tween.TRANS_CUBIC)
	seq.append(self, "angle", start, 2).set_trans(Tween.TRANS_CUBIC)
	seq.set_loops()

func _physics_process(delta):
# linear animation
#	angle = wrapf(angle + speed * delta, 0, TAU) # Infinite rotation (in radians)
	_update_platforms()
	
func _update_platforms():
	if platforms.size () == 0: return
	for i in platforms.size():
		var new_x = sin(angle) * spacing * (i + 1)
		var new_y = cos(angle) * spacing * (i + 1)
		platforms[i].position = Vector2(new_x, new_y)
		
func find_platforms():
	platforms = []
	for child in get_children(): platforms.append(child)
	if platforms.size() > 0:
		spacing = radius / platforms.size() # spacing between platforms (in pizels)

