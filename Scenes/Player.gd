extends KinematicBody2D

# TODO: salto coyote (permitir saltar cuando se acaba de caer)
# TODO: permitir pulsar el salto 0.2 segundos antes de llegar al suelo  
# TODO: efectos

const DEBUG_MAX_SPEED = false
const DEBUG_MOTION = false
const TIME_TO_MAX_SPEED = 0.2         # seconds to reach the max speed 0=immediate
const MAX_SPEED = 80                  # pixels/seconds
const STOP_IF_SPEED_IS_LESS_THAN = 20 # pixels/seconds
const MAX_FALLING_SPEED = 300         #

const FRICTION = 0.9                # 0=stop 0.9=10%/frame 0.99=ice!!
const AIR_RESISTANCE = 0.95        
const FLOOR = Vector2.UP
const SNAP_DIRECTION = Vector2.DOWN
const SNAP_LENGTH = 15

const stop_on_slopes = true

#const GRAVITY = 920
#const JUMP_FORCE = 368
var jumpHeight:float = 52;
var timeToJumpApex = 0.4;

onready var GRAVITY = (2 * jumpHeight) / pow(timeToJumpApex, 2)
onready var JUMP_FORCE = GRAVITY * timeToJumpApex;
onready var ACCELERATION = MAX_SPEED*1000 if TIME_TO_MAX_SPEED == 0 else MAX_SPEED/TIME_TO_MAX_SPEED


#The likely source of your problem is the combination of _physics_process and
#is_action_just_pressed. _physics_process is not guaranteed to be
#called every frame and so it can easily miss the action.

#A better solution would be to catch the jump in _input,
#store the information about the jump in a script-level variable
#and then look at this variable in _physics_process.


onready var sprite = $Sprite
onready var animationPlayer = $AnimationPlayer

var motion = Vector2.ZERO

var x_input:float;

func flip(left):
	sprite.flip_h = left;
	#sprite.scale.x = -1 if left else 1


func _process(delta):
	x_input = Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left");

var lastPos = 0
func lateral_movement(delta):
	if x_input != 0:
		if sign(motion.x) != sign(x_input): # si cambia de direccion de repente...
			# esto hace que se puede cambiar de direccion sin esperar a que decelere
			motion.x = x_input * ACCELERATION * delta
		else:
			motion.x += x_input * ACCELERATION * delta
		if abs(motion.x) >= MAX_SPEED:
			# test para saber cuando llega a la maxima velocidad
			animationPlayer.play("Run")
			
		flip(x_input < 0)
	else:
		# para suavamente
		animationPlayer.play("Stand")
		if abs(motion.x) < STOP_IF_SPEED_IS_LESS_THAN:
			motion.x = 0
		else:
			if is_on_floor():
				motion.x *= FRICTION
			else:
				motion.x *= AIR_RESISTANCE
	
func jump(delta):
	if is_on_floor():
		if Input.is_action_just_pressed("ui_up"):
			motion.y = -JUMP_FORCE
			return Vector2.ZERO
		#if Input.is_action_just_pressed("ui_down"):
	else:
		animationPlayer.play("Jump")
		if Input.is_action_just_released("ui_up") and motion.y < -JUMP_FORCE/2:
			motion.y = -JUMP_FORCE/2
	return SNAP_DIRECTION * SNAP_LENGTH

# Debug the motion change
var lastMotion = Vector2.ZERO

var lastStart = Vector2.ZERO
func _physics_process(delta):
	# delta is 1/60 = 0.01666 aprox
	# x * delta -> x / 60, move_and_slide no debe usar tiempos modificados con delta
	
	lateral_movement(delta)
	jump(delta)
	
	var snap_vector = jump(delta)
	
	motion.y += (GRAVITY * delta)
	motion.x = clamp(motion.x, -MAX_SPEED, MAX_SPEED)
	motion.y = min(motion.y, MAX_FALLING_SPEED) # avoid gravity continue forever in free fall

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
	
	lastMotion = motion
	var remainder = move_and_slide_with_snap(motion, snap_vector, FLOOR, stop_on_slopes)
	motion.y = remainder.y # this line stops the gravity accumulation
	if (!stop_on_slopes):
		motion.x = remainder.x 
	x_input = 0

