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
const FRICTION = 0.82                 # 0=stop 0.9=10%/frame 0.99=ice!!
const COYOTE_TIME = 0.1               # seconds. How much time the player can jump when falling 
const JUMP_HELPER_TIME = 120          # milliseconds. If the user press jump just before land
onready var ACCELERATION = MAX_SPEED*1000 if TIME_TO_MAX_SPEED == 0 else MAX_SPEED/TIME_TO_MAX_SPEED
const STOP_ON_SLOPES = true

# squeeze effect
const SQUEEZE_JUMP_TIME = 0.3                 # % correction per frame (lerp). The bigger, the faster
const SQUEEZE_JUMP = Vector2(0.5, 1.2)        # Vector to scale when jump
const SQUEEZE_LAND_TIME = 0.3                # % correction per frame (lerp). The bigger, the faster
const SQUEEZE_LAND = Vector2(1.2, 0.8)        # Vector to scale when land


# air
const JUMP_HEIGHT = 52                # jump max pixels
const MAX_JUMP_TIME = 0.4             # jump max time
const MAX_FALLING_SPEED = 300         # max speed in free fall
const AIR_RESISTANCE = 0.95        
const MAX_JUMPS = 1
onready var GRAVITY = (2 * JUMP_HEIGHT) / pow(MAX_JUMP_TIME, 2)
onready var JUMP_FORCE = GRAVITY * MAX_JUMP_TIME
onready var JUMP_FORCE_MIN = JUMP_FORCE / 2

# slope config
const FLOOR = Vector2.UP
const SNAP_LENGTH = 12                # be sure this value is less than the smallest tile
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
var movStartTime = 0

var jumps = 0
var canJump = false
var isJumping = false
var time_jump_pressed = 0

func flip(left):
	sprite.flip_h = left;
	#sprite.scale.x = -1 if left else 1

func _process(delta):
	var x_input = Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left")
	lateral_movement(x_input, delta)
	jump()
	
	if isJumping:
		animationPlayer.play("Jump")
	elif x_input != 0:
		animationPlayer.play("Run")
		flip(x_input < 0)
	else:
		animationPlayer.play("Stand")
		
	if !isJumping && Input.is_action_pressed("ui_down"):
		fall_from_platform()
		


func _ready():
	PlatformManager.subscribe_platform_out(self, "enable_platform_collide")
	enable_platform_collide()
	#Engine.set_target_fps(30)
	
func lateral_movement(x_input, delta):
	if x_input != 0:
		debug_acceleration()
		if sign(motion.x) != sign(x_input): # si cambia de direccion de repente...
			# esto hace que se puede cambiar de direccion sin esperar a que decelere
			motion.x = x_input * ACCELERATION * delta
		else:
			motion.x += x_input * ACCELERATION * delta
	else:
		# para suavamente
		if abs(motion.x) < STOP_IF_SPEED_IS_LESS_THAN: motion.x = 0
		else: motion.x *= FRICTION if is_on_floor() else AIR_RESISTANCE

func jump():
	if is_on_floor():
		jumps = 0
		canJump = true
		
	if Input.is_action_just_pressed("ui_up"):
		time_jump_pressed = OS.get_ticks_msec()
		
	if canJump:
		modulate = Color.white
		var now = OS.get_ticks_msec()
		if time_jump_pressed + JUMP_HELPER_TIME >= now:
			if SQUEEZE_JUMP_TIME != 0: sprite.scale = SQUEEZE_JUMP
			print("Jump:",JUMP_FORCE)
			motion.y = -JUMP_FORCE
			jumps = jumps + 1
			isJumping = true
			canJump = jumps < MAX_JUMPS
	else:
		# jumping or falling
		modulate = Color.red
		if Input.is_action_just_released("ui_up") and motion.y < -JUMP_FORCE_MIN:
			print("Jump from ",motion.y, " to ", JUMP_FORCE_MIN)
			motion.y = -JUMP_FORCE_MIN

func debug_acceleration():
	if DEBUG_ACCELERATION && motion.x != 0:
		if lastMotion.x == 0:
			movStartTime = OS.get_ticks_msec() # starts to move
		elif abs(motion.x) >= MAX_SPEED:
			if movStartTime != 0:
				print("FULL THROTLE IN "+str(OS.get_ticks_msec() - movStartTime))
			movStartTime = 0

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

# las plataformas desde la que se pueden bajar deben estar en el layer 2º (bit 1 pq empiezan por 0) y deben
# tener el collision mask vacio
# asi, el jugador puede desactivar el mask 2 (bit 1) y se cae
func fall_from_platform():
	PlatformManager.fall_from_platform(self)

func enable_platform_collide():
	PlatformManager.enable_platform_collide(self)

func restore_squeeze():
	if is_on_floor():
		if SQUEEZE_JUMP_TIME != 0:
			sprite.scale.y = lerp(sprite.scale.y, 1, SQUEEZE_JUMP_TIME)
			sprite.scale.x = lerp(sprite.scale.x, 1, SQUEEZE_JUMP_TIME)
	else:
		if SQUEEZE_LAND_TIME != 0:
			sprite.scale.y = lerp(sprite.scale.y, 1, SQUEEZE_LAND_TIME)
			sprite.scale.x = lerp(sprite.scale.x, 1, SQUEEZE_LAND_TIME)

func _physics_process(delta):
	
	var was_in_floor = is_on_floor()
	if !was_in_floor:
		# con esto se corrige el bug de que si STOP_ON_SLOPES es true, no se mueva junto a la plataforma
		motion.y += GRAVITY * delta

	motion.x = clamp(motion.x, -MAX_SPEED, MAX_SPEED)
	motion.y = min(motion.y, MAX_FALLING_SPEED) # avoid gravity continue forever in free fall

	debug_motion()

	if PlatformManager.is_falling_from_platform(self) || isJumping:
		var remain = move_and_slide(motion, Vector2.UP, true)
		motion.y = remain.y # this line stops the gravity accumulation
	else:
		var remain = move_and_slide_with_snap(motion, SLOPE_RAYCAST_VECTOR, FLOOR, STOP_ON_SLOPES)
		motion.y = remain.y # this line stops the gravity accumulation
		#motion.x = remain.x  # this line should be always commented, player can't climb slopes with it!!

	lastMotion = motion
	
	if is_on_ceiling():
		# slow down a little bit when the jump collides a celing
		motion.y = - GRAVITY * delta * 0.9
	
	if !was_in_floor && is_on_floor():
		# just grounded
		if SQUEEZE_LAND_TIME != 0: sprite.scale = SQUEEZE_LAND
		isJumping = false
		enable_platform_collide()
	elif was_in_floor && !is_on_floor() && !isJumping:
		# just falling!
		schedule_coyote_time()

	restore_squeeze()

func schedule_coyote_time():
	if COYOTE_TIME > 0:
		yield(get_tree().create_timer(COYOTE_TIME), "timeout")
	canJump = false

