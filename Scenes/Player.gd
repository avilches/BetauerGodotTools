extends KinematicBody2D

# TODO: salto coyote (permitir saltar cuando se acaba de caer)
# TODO: permitir pulsar el salto 0.2 segundos antes de llegar al suelo  
# TODO: efectos

const DEBUG_MAX_SPEED = false
const DEBUG_COLLISION = true
const DEBUG_MOTION = false
const DEBUG_ACCELERATION = false
const DEBUG_JUMP = false

# ground
const TIME_TO_MAX_SPEED = 0.2          # seconds to reach the max speed 0=immediate
const MAX_SPEED = 110                  # pixels/seconds
const STOP_IF_SPEED_IS_LESS_THAN = 20 # pixels/seconds
const FRICTION = 0.8                  # 0=stop 0.9=10%/frame 0.99=ice!!
const COYOTE_TIME = 0.1               # seconds. How much time the player can jump when falling 
const JUMP_HELPER_TIME = 0.2         # seconds. If the user press jump just before land
onready var ACCELERATION = MAX_SPEED*1000 if TIME_TO_MAX_SPEED == 0 else MAX_SPEED/TIME_TO_MAX_SPEED

# squeeze effect
const SQUEEZE_JUMP_TIME = 0.1                 # % correction per frame (lerp). The bigger, the faster
const SQUEEZE_JUMP = Vector2(1, 1.4)        # Vector to scale when jump
const SQUEEZE_LAND_TIME = 0.4                 # % correction per frame (lerp). The bigger, the faster
const SQUEEZE_LAND = Vector2(1.2, 0.8)        # Vector to scale when land


# air
const JUMP_HEIGHT = 80                # jump max pixels
const MAX_JUMP_TIME = 0.5             # jump max time
const MAX_FALLING_SPEED = 2000        # max speed in free fall
const AIR_RESISTANCE = 0.8              # 0=stop, 1=keep lateral movement until the end of the jump     
const MAX_JUMPS = 1
onready var GRAVITY = (2 * JUMP_HEIGHT) / pow(MAX_JUMP_TIME, 2)
onready var JUMP_FORCE = GRAVITY * MAX_JUMP_TIME
onready var JUMP_FORCE_MIN = JUMP_FORCE / 2

# slope config
const FLOOR = Vector2.UP
const SLOW_DOWN_ON_SLOPE_PERCENT = 0.8 # % speed slow down in slopes. 1 = no slow down, 0.5 = half
const SNAP_LENGTH = 12                 # be sure this value is less than the smallest tile
onready var SLOPE_RAYCAST_VECTOR = Vector2.DOWN * SNAP_LENGTH

onready var spriteHolder = $Sprites
onready var spriteRun = $Sprites/Run
onready var spriteIdle = $Sprites/Idle
onready var spriteJump = $Sprites/Jump
onready var spriteFall = $Sprites/Fall
onready var sprite = spriteIdle


var motion = Vector2.ZERO
var x_input:float = 0;
var y_input:float = 0;

# Debug the motion change
var movStartPosMAXSPEED = 0
var lastMotion = Vector2.ZERO
var movStartTimeMAXSPEED = Vector2.ZERO
var movStartTimeACC = -1

var jumps = 0
var canJump = false
var isJumping = false
var time_jump_pressed = -1

func flip(left):
	sprite.flip_h = !left;

func change_sprite(nextSprite):
	if sprite == nextSprite:
		return
	sprite.visible = false
	nextSprite.visible = true
	nextSprite.flip_h = sprite.flip_h  # keep the current flip state
	sprite = nextSprite

func update_sprite(delta, x_input):
	if isJumping:
		change_sprite(spriteJump)
		if sign(motion.y) == 1:
			change_sprite(spriteFall)
	elif x_input != 0:
		change_sprite(spriteRun)
		
	else:
		if sign(motion.y) == 1:
			change_sprite(spriteFall)
		else:
			change_sprite(spriteIdle)

	if x_input != 0:
		flip(x_input < 0)
		
func _ready():
	if !GameManager.isPlayer(self):
		var msg = "Player node name should be "+GameManager.PLAYER_NAME+" (current name: "+self.name+")"
		print(msg)
		push_error(msg)
		
	PlatformManager.subscribe_platform_out(self, "stop_falling_from_platform")
	stop_falling_from_platform()
	for sp in $Sprites.get_children(): sp.visible = false
	
	GameManager.connect("death", self, "on_death")
	
	
func on_death(cause):
	print("MUETO")
	set_process(false)
	set_physics_process(false)
	#Engine.set_target_fps(30)
	
func lateral_movement(x_input, delta):
	if x_input != 0:
		if sign(motion.x) != sign(x_input): # si cambia de direccion de repente...
			# esto hace que se puede cambiar de direccion sin esperar a que decelere
			motion.x = x_input * ACCELERATION * delta
		else:
			motion.x += x_input * ACCELERATION * delta
	else:
		# para suavamente
		if abs(motion.x) < STOP_IF_SPEED_IS_LESS_THAN: motion.x = 0
		else: motion.x *= FRICTION if is_on_floor() else AIR_RESISTANCE

func jump(delta):
	if is_on_floor():
		jumps = 0
		canJump = true
		
	if Input.is_action_just_pressed("ui_up"):
		time_jump_pressed = 0
	elif time_jump_pressed != -1:
		time_jump_pressed += delta
		
	if canJump:
		if time_jump_pressed != -1 && time_jump_pressed <= JUMP_HELPER_TIME:
			if DEBUG_JUMP:
				if (time_jump_pressed > 0): print("Jump helper: ", time_jump_pressed, "s (Config:", JUMP_HELPER_TIME,")")
				print("Jump start:", -JUMP_FORCE)
			time_jump_pressed = -1
			if SQUEEZE_JUMP_TIME != 0: spriteHolder.scale = SQUEEZE_JUMP
			
			motion.y = -JUMP_FORCE
			jumps = jumps + 1
			isJumping = true
			canJump = jumps < MAX_JUMPS
			# TODO: controlar que el segundo salto se haga justo arriba, no al llegar al suelo
			#       para que no coincida con el helper...
	
	
	if Input.is_action_just_released("ui_up") && motion.y < -JUMP_FORCE_MIN && isJumping:
		if DEBUG_JUMP:
			print("Short jump: deccelerating from ", motion.y, " to ", -JUMP_FORCE_MIN)
		motion.y = -JUMP_FORCE_MIN

func debug_motion(delta):
	if DEBUG_ACCELERATION && motion.x != 0:
		if lastMotion.x == 0:
			movStartTimeACC = 0 # starts to move
		elif movStartTimeACC != -1:
			movStartTimeACC += delta
			if abs(motion.x) >= MAX_SPEED:
				print("Full throtle ", motion.x, " in ", movStartTimeACC, "s")
				movStartTimeACC = -1

	if DEBUG_MAX_SPEED:
		if motion.x != 0:
			if lastMotion.x == 0:
				movStartTimeMAXSPEED = 0
				movStartPosMAXSPEED = get_position().x
			else:
				movStartTimeMAXSPEED += delta
				if movStartTimeMAXSPEED >= 1 && movStartPosMAXSPEED != -1:
					var distance = get_position().x - movStartPosMAXSPEED
					print("Moved from ", movStartPosMAXSPEED, " to ",  get_position().x, " in ", movStartTimeMAXSPEED, "s. Speed: ", abs(round(distance)),"px/second")
					movStartTimeMAXSPEED = -1
		else:
			movStartPosMAXSPEED = 0

	if DEBUG_MOTION && (lastMotion.x != motion.x || lastMotion.y != motion.y): print(motion)

# las plataformas desde la que se pueden bajar deben estar en el layer 2º (bit 1 pq empiezan por 0) y deben
# tener el collision mask vacio
# asi, el jugador puede desactivar el mask 2 (bit 1) y se cae
func fall_from_platform():
	PlatformManager.fall_from_platform(self)

func stop_falling_from_platform():
	PlatformManager.stop_falling_from_platform(self)

func is_on_slope() -> bool:
	if is_on_floor():
#		if get_slide_count() == 0: print("NO COLLISION") 
		for i in get_slide_count():
			var collision = get_slide_collision(i)
			if abs(collision.normal.y) < 1:
				return true
	return false
	
func is_on_moving_platform() -> bool:
	if is_on_floor():
#		if get_slide_count() == 0: print("NO COLLISION") 
		for i in get_slide_count():
			var collision = get_slide_collision(i)
			if collision.collider is KinematicBody2D && PlatformManager.is_moving_platform(collision.collider):
				return true
	return false

func debug_collision():
	if DEBUG_COLLISION && get_slide_count():
		print("Body:",int(get_collision_mask_bit(0)), int(get_collision_mask_bit(1)), int(get_collision_mask_bit(2)))
		for i in get_slide_count():
			var collision = get_slide_collision(i)
			print("Coll:",int(collision.collider.get_collision_layer_bit(0)), int(collision.collider.get_collision_layer_bit(1)), int(collision.collider.get_collision_layer_bit(2)), " ", collision.collider.get_class(), ":'", collision.collider.name+"'")
		
	
func _physics_process(delta):

	var x_input = Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left")
	lateral_movement(x_input, delta)
	jump(delta)

	var applyGravity = true
	if applyGravity:
		motion.y += GRAVITY * delta

	var realMaxSpeed = MAX_SPEED * 1.5 if Input.is_action_pressed("ui_accept") else MAX_SPEED
	motion.x = clamp(motion.x, -realMaxSpeed, realMaxSpeed)
	motion.y = min(motion.y, MAX_FALLING_SPEED) # avoid gravity continue forever in free fall

	debug_motion(delta)

	var was_in_floor = is_on_floor()
	var slowdownVector = Vector2(1, SLOW_DOWN_ON_SLOPE_PERCENT) if is_on_slope() else Vector2.ONE
	if PlatformManager.is_falling_from_platform(self) || isJumping:
		# STOP_ON_SLOPES debe ser true para al caer sobre una pendiente la tome como suelo
		var STOP_ON_SLOPES = true
		var remain = move_and_slide_with_snap(motion * slowdownVector, Vector2.ZERO, FLOOR, STOP_ON_SLOPES)
		motion.y = remain.y # this line stops the gravity accumulation
		
		# inertia false = se mantiene el remain.x = al chocar con la cabeza pierde toda la inercia lateral que tenia y se va para abajo. Y si choca al subir y se
		# se sube, pierde tambien la inercia teniendo que alecerar desde 0
		# intertia true = se pierde el remain.x = al saltar y chocar (temporalmente) con un objeto hace que al dejar de chocar, recupere
		# totalmente la movilidad = si choca justo antes de subir y luego se sube, corre a tope. Si choca con la cabeza y baja un poco,
		# cuando de chocar, continua hacia delante a tope.
		var inertia = false
		
		if !inertia:
			motion.x = remain.x

	else:
		# STOP_ON_SLOPES debe ser true para que no se resbale en las pendientes, pero tiene que ser false si se mueve una plataforma y nos queremos quedar pegados
		# Bug: si la plataforma que se mueve tiene slope, entonces para que detected el slope como suelo, se para y ya no sigue a la plataforma
		var STOP_ON_SLOPES = get_floor_velocity() == Vector2.ZERO || is_on_slope()
		var remain = move_and_slide_with_snap(motion * slowdownVector, SLOPE_RAYCAST_VECTOR, FLOOR, STOP_ON_SLOPES)
		motion.y = remain.y  # this line stops the gravity accumulation
#		motion.x = remain.x  # this line should be always commented, player can't climb slopes with it!!
		
	lastMotion = motion
	
#	if is_on_ceiling():
		# slow down a little bit when the jump collides a celing
#		motion.y = - GRAVITY * delta * 0.9
	

	
	if !was_in_floor && is_on_floor():
		debug_collision()
		# just grounded
		if SQUEEZE_LAND_TIME != 0: spriteHolder.scale = SQUEEZE_LAND
		isJumping = false
		stop_falling_from_platform()
	elif was_in_floor && !is_on_floor() && !isJumping:
		# just falling!
		schedule_coyote_time()

	if !isJumping && Input.is_action_pressed("ui_down"):
		fall_from_platform()

	update_sprite(delta, x_input)
	restore_squeeze()

func schedule_coyote_time():
	if COYOTE_TIME > 0:
		yield(get_tree().create_timer(COYOTE_TIME), "timeout")
	canJump = false

func restore_squeeze():
	if is_on_floor():
		if SQUEEZE_LAND_TIME != 0:
			spriteHolder.scale.y = lerp(spriteHolder.scale.y, 1, SQUEEZE_LAND_TIME)
			spriteHolder.scale.x = lerp(spriteHolder.scale.x, 1, SQUEEZE_LAND_TIME)
	else:
		if SQUEEZE_JUMP_TIME != 0:
			spriteHolder.scale.y = lerp(spriteHolder.scale.y, 1, SQUEEZE_JUMP_TIME)
			spriteHolder.scale.x = lerp(spriteHolder.scale.x, 1, SQUEEZE_JUMP_TIME)
