extends Reference
class_name CharacterConfig





# CONFIG: debug
var DEBUG_MAX_SPEED = false
var DEBUG_ACCELERATION = false
var DEBUG_SLOPE_STAIRS = false
var DEBUG_COLLISION = false
var DEBUG_MOTION = false
var DEBUG_JUMP = false

# CONFIG: ground
var MAX_SPEED = 120                  # pixels/seconds
var ACCELERATION = 120               # pixels/frame
var STOP_IF_SPEED_IS_LESS_THAN = 20  # pixels/seconds
var FRICTION = 0                     # 0=stop immediately, 0.9=10%/frame 0.99=ice!!


# CONFIG: air
var GRAVITY
var JUMP_FORCE
var JUMP_FORCE_MIN


var MAX_FALLING_SPEED = 2000        # max speed in free fall
var START_FALLING_SPEED = 100       # speed where the player changes to falling (test with fast downwards platform!)
var AIR_RESISTANCE = 0              # 0=stop immediately, 1=keep lateral movement until the end of the jump






func configure_speed(_MAX_SPEED, TIME_TO_MAX_SPEED = 0):
	MAX_SPEED = _MAX_SPEED
	ACCELERATION = MAX_SPEED/(TIME_TO_MAX_SPEED + 0.0000001) # avoid divide by zero

func configure_jump(JUMP_HEIGHT, MAX_JUMP_TIME):
	GRAVITY = (2 * JUMP_HEIGHT) / pow(MAX_JUMP_TIME, 2)
	JUMP_FORCE = GRAVITY * MAX_JUMP_TIME
	JUMP_FORCE_MIN = JUMP_FORCE / 2
