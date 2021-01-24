extends CharacterConfig
class_name PlayerConfig

const COYOTE_TIME = 0.1               # seconds. How much time the player can jump when falling
const JUMP_HELPER_TIME = 0.2          # seconds. If the user press jump just before land
const MAX_JUMPS = 1

func _init():
	#DEBUG_MAX_SPEED = true
	#DEBUG_ACCELERATION = true
	#DEBUG_SLOPE_STAIRS = true
	#DEBUG_COLLISION = true
	#DEBUG_MOTION = true
	#DEBUG_JUMP = true

	var MAX_SPEED = 110.0            # pixels/seconds
	var TIME_TO_MAX_SPEED = 0.2      # seconds to reach the max speed 0=immediate
	configure_speed(MAX_SPEED, TIME_TO_MAX_SPEED)
	STOP_IF_SPEED_IS_LESS_THAN = 20  # pixels/seconds
	FRICTION = 0.8                   # 0=stop immediately 0.9=10%/frame 0.99=ice!!


	# CONFIG: air
	var JUMP_HEIGHT = 80                # jump max pixels
	var MAX_JUMP_TIME = 0.5             # jump max time
	configure_jump(JUMP_HEIGHT, MAX_JUMP_TIME)
	JUMP_FORCE_MIN = JUMP_FORCE / 2

	MAX_FALLING_SPEED = 2000        # max speed in free fall
	START_FALLING_SPEED = 1         # speed where the player changes to falling (test with fast downwards platform!)
	AIR_RESISTANCE = 0              # 0=stop immediately, 1=keep lateral movement until the end of the jump

# CONFIG: squeeze effect
var SQUEEZE_JUMP_TIME = 0.1                 # % correction per frame (lerp). The bigger, the faster
var SQUEEZE_JUMP_SCALE = Vector2(1, 1.4)    # Vector to scale when jump
var SQUEEZE_LAND_TIME = 0.4                 # % correction per frame (lerp). The bigger, the faster
var SQUEEZE_LAND_SCALE = Vector2(1.2, 0.8)  # Vector to scale when land
