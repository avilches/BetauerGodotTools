extends FallMovingPlatform

export var radius = Vector2(50, 50)
export var rotation_duration = 4.0

var speed = TAU / rotation_duration # speed is radians/seconds. TAU is a whole round in one second
var angle = 0

func _physics_process(delta):
	angle = wrapf(angle + speed * delta, 0, TAU) # Infinite rotation (in radians)
	var x = sin(angle) * radius.x
	var y = cos(angle) * radius.y
	position = Vector2(x, y)
