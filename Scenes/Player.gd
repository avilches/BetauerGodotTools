extends KinematicBody2D

# TODO: salto coyote (permitir saltar cuando se acaba de caer)
# TODO: permitir pulsar el salto 0.2 segundos antes de llegar al suelo  
# TODO: efectos

const DEBUG_MAX_SPEED = false
const DEBUG_MOTION = false
const DEBUG_ACCELERATION = true

# ground
const TIME_TO_MAX_SPEED = 0.2         # seconds to reach the max speed 0=immediate
const MAX_SPEED = 80                  # pixels/seconds
const STOP_IF_SPEED_IS_LESS_THAN = 20 # pixels/seconds
const FRICTION = 0.9                  # 0=stop 0.9=10%/frame 0.99=ice!!
onready var ACCELERATION = MAX_SPEED*1000 if TIME_TO_MAX_SPEED == 0 else MAX_SPEED/TIME_TO_MAX_SPEED

# air
const JUMP_HEIGHT = 52                # jump max pixels
const MAX_JUMP_TIME = 0.4             # jump max time
const MAX_FALLING_SPEED = 300         # max speed in free fall
const AIR_RESISTANCE = 0.95        
onready var GRAVITY = (2 * JUMP_HEIGHT) / pow(MAX_JUMP_TIME, 2)
onready var JUMP_FORCE = GRAVITY * MAX_JUMP_TIME

# slope config
const STOP_ON_SLOPES = true
const FLOOR = Vector2.UP
const SNAP_LENGTH = 15                # be sure this value is less than the smallest tile
onready var SLOPE_RAYCAST_VECTOR = Vector2.DOWN * SNAP_LENGTH

onready var sprite = $Sprite
onready var animationPlayer = $AnimationPlayer

var motion = Vector2.ZERO
var x_input:float = 0;
var y_input:float = 0;
# Debug the motion change
var lastPos = 0
var lastMotion = Vector2.ZERO
var lastStart = Vector2.ZERO
var lastAcc = 0

func flip(left):
	sprite.flip_h = left;
	#sprite.scale.x = -1 if left else 1

#func _process(delta):
#func _ready():
#	Engine.set_target_fps(30)
	
func lateral_movement(delta):
	var x_input = Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left")

	if x_input != 0:
		if lastMotion.x == 0: lastAcc = OS.get_ticks_msec() # starts to move
		if sign(motion.x) != sign(x_input): # si cambia de direccion de repente...
			# esto hace que se puede cambiar de direccion sin esperar a que decelere
			motion.x = x_input * ACCELERATION * delta
		else:
			motion.x += x_input * ACCELERATION * delta
		if abs(motion.x) >= MAX_SPEED:
			if lastAcc > 0 && DEBUG_ACCELERATION: print("FULL THROTLE IN "+str(OS.get_ticks_msec()-lastAcc))
			lastAcc = 0
		animationPlayer.play("Run")
		flip(x_input < 0)
	else:
		# para suavamente
		animationPlayer.play("Stand")
		if abs(motion.x) < STOP_IF_SPEED_IS_LESS_THAN: motion.x = 0
		else: motion.x *= FRICTION if is_on_floor() else AIR_RESISTANCE

func jump():
	if is_on_floor():
		if Input.is_action_just_pressed("ui_up"):
			motion.y = -JUMP_FORCE
			return Vector2.ZERO
	else:
		animationPlayer.play("Jump")
		if Input.is_action_just_released("ui_up") and motion.y < -JUMP_FORCE/2:
			motion.y = -JUMP_FORCE/2

	return SLOPE_RAYCAST_VECTOR

func debug_motion():
	if DEBUG_MAX_SPEED:
		if motion.x != 0:
			if lastMotion.x == 0:
				lastStart = OS.get_ticks_msec()
				lastPos = get_position().x
			else:
				var elapsed = OS.get_ticks_msec() - lastStart
				if elapsed > 1000 && lastPos != 0:
					var distance = str(get_position().x - lastPos)
					print("Moved from "+str(get_position().x)+" to "+str(lastPos)+": "+str(distance)+"/second")
					lastStart = OS.get_ticks_msec()
					lastPos = get_position().x
		else:
			lastPos = 0

	if DEBUG_MOTION && (lastMotion.x != motion.x || lastMotion.y != motion.y): print(motion)

func _physics_process(delta):

	lateral_movement(delta)
	var snap_vector = jump()

	motion.y += GRAVITY * delta
	motion.x = clamp(motion.x, -MAX_SPEED, MAX_SPEED)
	motion.y = min(motion.y, MAX_FALLING_SPEED) # avoid gravity continue forever in free fall

	debug_motion()

	lastMotion = motion
	var remain = move_and_slide_with_snap(motion, snap_vector, FLOOR, STOP_ON_SLOPES)
	motion.y = remain.y # this line stops the gravity accumulation
	if (!STOP_ON_SLOPES): motion.x = remain.x

