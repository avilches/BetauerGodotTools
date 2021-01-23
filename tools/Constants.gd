extends Node

# CONFIG: debug
const DEBUG_MAX_SPEED = false
const DEBUG_ACCELERATION = false

const DEBUG_COLLISION = false
const DEBUG_MOTION = false
const DEBUG_JUMP = false

# CONFIG: ground
const TIME_TO_MAX_SPEED = 0.2          # seconds to reach the max speed 0=immediate
var ACCELERATION = MAX_SPEED/(TIME_TO_MAX_SPEED + 0.0000001) # avoid divide by zero 
const MAX_SPEED = 110.0                # pixels/seconds
const STOP_IF_SPEED_IS_LESS_THAN = 20 # pixels/seconds
const FRICTION = 0.8                  # 0=stop 0.9=10%/frame 0.99=ice!!
const COYOTE_TIME = 0.1               # seconds. How much time the player can jump when falling
const JUMP_HELPER_TIME = 0.2         # seconds. If the user press jump just before land


# CONFIG: air
const JUMP_HEIGHT = 80                # jump max pixels
const MAX_JUMP_TIME = 0.5             # jump max time
const MAX_FALLING_SPEED = 2000        # max speed in free fall
const START_FALLING_SPEED = 100       # speed where the player changes to falling (test with fast downwards platform!)
const AIR_RESISTANCE = 0.8            # 0=stop, 1=keep lateral movement until the end of the jump
const MAX_JUMPS = 1
const GRAVITY = (2 * JUMP_HEIGHT) / pow(MAX_JUMP_TIME, 2)
const JUMP_FORCE = GRAVITY * MAX_JUMP_TIME
const JUMP_FORCE_MIN = JUMP_FORCE / 2

# CONFIG: slope config
const FLOOR = Vector2.UP
const SLOW_ON_SLOPE_DOWN = 0.4     # % speed slow % in slopes. 1 = no slow down, 0.5 = half
const SLOW_ON_SLOPE_UP = 0.9       # % speed slow % in slopes. 1 = no slow down, 0.5 = half
const SNAP_LENGTH = 12             # be sure this value is less than the smallest tile
const SLOPE_RAYCAST_VECTOR = Vector2.DOWN * SNAP_LENGTH

# CONFIG: squeeze effect
const SQUEEZE_JUMP_TIME = 0.1                 # % correction per frame (lerp). The bigger, the faster
const SQUEEZE_JUMP_SCALE = Vector2(1, 1.4)    # Vector to scale when jump
const SQUEEZE_LAND_TIME = 0.4                 # % correction per frame (lerp). The bigger, the faster
const SQUEEZE_LAND_SCALE = Vector2(1.2, 0.8)  # Vector to scale when land

