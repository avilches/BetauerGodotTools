extends KinematicBody2D

# TODO: efectos
# escalar a las paredes
# engancharse en las repisas de las plataformas

# CONFIG: debug
const DEBUG_MAX_SPEED = false
const DEBUG_ACCELERATION = false

const DEBUG_COLLISION = false
const DEBUG_MOTION = false
const DEBUG_JUMP = false

# CONFIG: ground
const TIME_TO_MAX_SPEED = 0.2          # seconds to reach the max speed 0=immediate
const MAX_SPEED = 110.0                # pixels/seconds
const STOP_IF_SPEED_IS_LESS_THAN = 20 # pixels/seconds
const FRICTION = 0.8                  # 0=stop 0.9=10%/frame 0.99=ice!!
const COYOTE_TIME = 0.1               # seconds. How much time the player can jump when falling 
const JUMP_HELPER_TIME = 0.2         # seconds. If the user press jump just before land
onready var ACCELERATION = MAX_SPEED*1000.0 if TIME_TO_MAX_SPEED == 0 else MAX_SPEED/TIME_TO_MAX_SPEED

# CONFIG: air
const JUMP_HEIGHT = 80                # jump max pixels
const MAX_JUMP_TIME = 0.5             # jump max time
const MAX_FALLING_SPEED = 2000        # max speed in free fall
const START_FALLING_SPEED = 100       # speed where the player changes to falling (test with fast downwards platform!)
const AIR_RESISTANCE = 0.8            # 0=stop, 1=keep lateral movement until the end of the jump
const MAX_JUMPS = 1
onready var GRAVITY = (2 * JUMP_HEIGHT) / pow(MAX_JUMP_TIME, 2)
onready var JUMP_FORCE = GRAVITY * MAX_JUMP_TIME
onready var JUMP_FORCE_MIN = JUMP_FORCE / 2

# CONFIG: slope config
const FLOOR = Vector2.UP
const SLOW_ON_SLOPE_DOWN = 0.4     # % speed slow % in slopes. 1 = no slow down, 0.5 = half
const SLOW_ON_SLOPE_UP = 0.9       # % speed slow % in slopes. 1 = no slow down, 0.5 = half
const SNAP_LENGTH = 12             # be sure this value is less than the smallest tile
onready var SLOPE_RAYCAST_VECTOR = Vector2.DOWN * SNAP_LENGTH

# CONFIG: squeeze effect
const SQUEEZE_JUMP_TIME = 0.1                 # % correction per frame (lerp). The bigger, the faster
const SQUEEZE_JUMP_SCALE = Vector2(1, 1.4)    # Vector to scale when jump
const SQUEEZE_LAND_TIME = 0.4                 # % correction per frame (lerp). The bigger, the faster
const SQUEEZE_LAND_SCALE = Vector2(1.2, 0.8)  # Vector to scale when land

onready var spriteHolder = $Sprites
onready var spriteRun = $Sprites/Run
onready var spriteIdle = $Sprites/Idle
onready var spriteJump = $Sprites/Jump
onready var spriteFall = $Sprites/Fall
onready var sprite = spriteIdle

var motion = Vector2.ZERO
var jumps = 0
var canJump = false
var isJumping = false
var time_jump_pressed = -1

# Updated manually before move_and.. by update_ground_colliders()
var is_on_slope
var is_on_moving_platform
var is_on_falling_platform
var is_on_slope_stairs
# Changed by signals
var is_on_slope_stairs_down = false
var is_on_slope_stairs_up = false

# Variables only to debug
var colliderNormal:Vector2
var movStartPosMAXSPEED
var lastMotion = Vector2.ZERO
var movStartTimeMAXSPEED = Vector2.ZERO
var movStartTimeACC = -1

func _ready():
	if !GameManager.isPlayer(self):
		var msg = "Player node name should be "+GameManager.PLAYER_NAME+" (current name: "+self.name+")"
		print(msg)
		push_error(msg)
		
	PlatformManager.subscribe_platform_out(self, "stop_falling_from_platform")
	
	PlatformManager.on_slope_stairs_down_flag(self, "is_on_slope_stairs_down")
	PlatformManager.on_slope_stairs_up_flag(self, "is_on_slope_stairs_up")
	#PlatformManager.subscribe_slope_stairs_up(self, "_slope_stairs_up_in", "_slope_stairs_up_out")
	#PlatformManager.subscribe_slope_stairs_down(self, "_slope_stairs_down_in", "_slope_stairs_down_out")

	PlatformManager.subscribe_slope_stairs_enabler(self, "_slope_stairs_enabler_in", "_slope_stairs_enabler_out")
	PlatformManager.subscribe_slope_stairs_disabler(self, "_slope_stairs_disabler_in", "_slope_stairs_disabler_out")
	
	stop_falling_from_platform()
	for sp in $Sprites.get_children(): sp.visible = false
	
	GameManager.connect("death", self, "on_death")

func fall_from_platform():
	if is_on_falling_platform:
		PlatformManager.body_fall_from_platform(self)
		debug_player_masks()

func stop_falling_from_platform():
	PlatformManager.body_stop_falling_from_platform(self)
	debug_player_masks()

func slope_stairs_up_in(body, _area2D):
	if body == self:
		print("Entering up hall...")

func slope_stairs_up_out(body, _area2D):
	if body == self:
		print("Exiting up hall...")

#func _slope_stairs_up_in(body, area2D):
#	if body == self: print("stairs_up_in")
#
#func _slope_stairs_up_out(body, area2D):
#	if body == self: print("stairs_up_out")
#
#func _slope_stairs_down_in(body, area2D):
#	if body == self: print("stairs_down_in")
#
#func _slope_stairs_down_out(body, area2D):
#	if body == self: print("stairs_down_out")

func _slope_stairs_enabler_in(body, _area2D):
	if body == self:
		print("stairs_enabler_in")
		enable_slope_stairs()


func _slope_stairs_enabler_out(body, _area2D):
	if body == self: print("stairs_enabler_out")

func _slope_stairs_disabler_in(body, _area2D):
	if body == self:
		print("stairs_disabler_in")
		disable_slope_stairs()

func _slope_stairs_disabler_out(body, _area2D):
	if body == self: print("stairs_disabler_out")

func on_death(_cause):
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
		
	if Input.is_action_just_pressed("ui_select"):
		time_jump_pressed = 0
	elif time_jump_pressed != -1:
		time_jump_pressed += delta
		
	if canJump:
		if time_jump_pressed != -1 && time_jump_pressed <= JUMP_HELPER_TIME:
			if DEBUG_JUMP:
				if (time_jump_pressed > 0): print("Jump helper: ", time_jump_pressed, "s (Config:", JUMP_HELPER_TIME,")")
				print("Jump start:", -JUMP_FORCE)
			time_jump_pressed = -1
			if SQUEEZE_JUMP_TIME != 0: spriteHolder.scale = SQUEEZE_JUMP_SCALE
			
			motion.y = -JUMP_FORCE
			jumps = jumps + 1
			isJumping = true
			canJump = jumps < MAX_JUMPS
			# TODO: controlar que el segundo salto se haga justo arriba, no al llegar al suelo
			#       para que no coincida con el helper...
	
	
	if Input.is_action_just_released("ui_select") && motion.y < -JUMP_FORCE_MIN && isJumping:
		if DEBUG_JUMP:
			print("Short jump: deccelerating from ", motion.y, " to ", -JUMP_FORCE_MIN)
		motion.y = -JUMP_FORCE_MIN
	
func update_ground_colliders():
	is_on_slope = false
	is_on_moving_platform = false
	is_on_falling_platform = false
	is_on_slope_stairs = false
	colliderNormal = Vector2.ZERO
	if !is_on_floor(): return
#	if get_slide_count() == 0: print("Ground but no colliders??") 
	for i in get_slide_count():
		var collision = get_slide_collision(i)
		colliderNormal = collision.normal
		if collision.collider.has_method("collide_with"):
			collision.collider.collide_with(self, collision)
		
		if abs(collision.normal.y) < 1:
			is_on_slope = true
		
		if collision.collider is KinematicBody2D && PlatformManager.is_a_moving_platform(collision.collider):
			is_on_moving_platform = true
		
		if collision.collider is PhysicsBody2D && PlatformManager.is_a_falling_platform(collision.collider):
			is_on_falling_platform = true

		if collision.collider is PhysicsBody2D && PlatformManager.is_a_slope_stairs(collision.collider):
			is_on_slope_stairs = true
	update()
		

var applyGravity = true

func enable_slope_stairs():
	PlatformManager.body_disable_slope_stairs_cover(self)
	PlatformManager.body_enable_slope_stairs(self)
	
func disable_slope_stairs():
	PlatformManager.body_enable_slope_stairs_cover(self)
	PlatformManager.body_disable_slope_stairs(self)

func _physics_process(delta):
	
	$Label.text = str(PlatformManager.body_has_slope_stairs_enabled(self), " ", PlatformManager.body_has_slope_stairs_cover_enabled(self))
#	$Label.text = str(is_on_slope_stairs_down, " ", is_on_slope_stairs_up)

	var x_input = Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left")
	lateral_movement(x_input, delta)
	jump(delta)

	if applyGravity:
		motion.y += GRAVITY * delta

	var realMaxSpeed = MAX_SPEED * 1.5 if Input.is_action_pressed("ui_accept") else MAX_SPEED
	motion.x = clamp(motion.x, -realMaxSpeed, realMaxSpeed)
	motion.y = min(motion.y, MAX_FALLING_SPEED) # avoid gravity continue forever in free fall

	debug_motion(delta)

	var slowdownVector = Vector2.ONE
	var slope_down = null
	
	if is_on_slope && !isJumping && x_input != 0:
		slope_down = sign(colliderNormal.x) == sign(x_input) # pendiente y direccion al mismo lado
		slowdownVector = SLOW_ON_SLOPE_DOWN if slope_down else SLOW_ON_SLOPE_UP
	
	if is_on_slope_stairs_down:
		if Input.is_action_pressed("ui_up") && !isJumping:
			enable_slope_stairs()
		elif x_input != 0:
			disable_slope_stairs()
	elif is_on_slope_stairs_up:
		if Input.is_action_pressed("ui_down") && !isJumping:
			enable_slope_stairs()
		elif x_input != 0 :
			disable_slope_stairs()

	lastMotion = motion
	var was_in_floor = is_on_floor()
	var was_on_slope = is_on_slope
	var was_on_falling_platform = is_on_falling_platform
	var was_on_moving_platform = is_on_moving_platform
	var was_on_slope_stairs = is_on_slope_stairs

	if PlatformManager.is_body_falling_from_platform(self) || isJumping:
		# STOP_ON_SLOPES debe ser true para al caer sobre una pendiente la tome comoelo
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
		var LATERAL_MOVEMENT_PLATFORM = get_floor_velocity().x != 0
		var STOP_ON_SLOPES = !LATERAL_MOVEMENT_PLATFORM
		var remain = move_and_slide_with_snap(motion * slowdownVector, SLOPE_RAYCAST_VECTOR, FLOOR, STOP_ON_SLOPES)
		motion.y = remain.y  # this line stops the gravity accumulation
#		motion.x = remain.x  # this line should be always commented, player can't climb slopes with it!!
		
	update_ground_colliders()
	
	if !was_in_floor && is_on_floor():
		debug_collision()
		# just grounded
		if SQUEEZE_LAND_TIME != 0: spriteHolder.scale = SQUEEZE_LAND_SCALE
		isJumping = false
		stop_falling_from_platform()
		if is_on_slope and x_input == 0:
			# Evita resbalarse un poco Al caer sobre un slope en linea recta
			motion.x = 0
#			motion.y = 0
			
	elif was_in_floor && !is_on_floor() && !isJumping:
		# just falling!
		schedule_coyote_time()

	if !isJumping && Input.is_action_pressed("ui_down"):
		fall_from_platform()

	update_sprite(delta, x_input)
	restore_squeeze()
	if motion.x == 0:
		# pixer perfect only stopped (TODO: only do that once, not every frame)
		position = Vector2(round(position.x), round(position.y))

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

func flip(left):
	sprite.flip_h = !left;

func change_sprite(nextSprite):
	if sprite == nextSprite:
		return
	sprite.visible = false
	nextSprite.visible = true
	nextSprite.flip_h = sprite.flip_h  # keep the current flip state
	sprite = nextSprite

func update_sprite(_delta, x_input):
	if isJumping:
		change_sprite(spriteJump)
		if sign(motion.y) == 1:
			change_sprite(spriteFall)
	elif x_input != 0:
		change_sprite(spriteRun)
		
	else:
		if motion.y > START_FALLING_SPEED:
			change_sprite(spriteFall)
		else:
			change_sprite(spriteIdle)

	if x_input != 0:
		flip(x_input < 0)


###########################################################################
# DEBUG
###########################################################################

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
				movStartPosMAXSPEED = get_position()
			else:
				if movStartTimeMAXSPEED >= 1:
					var distance = get_position().distance_to(movStartPosMAXSPEED)
					# No funciona bien si se cambia de direccion...
					print("Moved from ", movStartPosMAXSPEED, " to ",  get_position(), " in ", movStartTimeMAXSPEED, "s. Speed: ", abs(round(distance)),"px/second")
					movStartTimeMAXSPEED = 0
					movStartPosMAXSPEED = get_position()
				else:
					movStartTimeMAXSPEED += delta
					
		else:
			movStartPosMAXSPEED = null

	if DEBUG_MOTION && (lastMotion.x != motion.x || lastMotion.y != motion.y): print(motion, motion-lastMotion)

func debug_player_masks():
	if DEBUG_COLLISION:
		print("Player:  ",int(get_collision_mask_bit(0)), int(get_collision_mask_bit(1)), int(get_collision_mask_bit(2)))
		
func debug_collision():
	if DEBUG_COLLISION && get_slide_count():
		debug_player_masks()
		for i in get_slide_count():
			var collision = get_slide_collision(i)
			print("Collider:",int(collision.collider.get_collision_layer_bit(0)), int(collision.collider.get_collision_layer_bit(1)), int(collision.collider.get_collision_layer_bit(2)), " ", collision.collider.get_class(), ":'", collision.collider.name+"'")

func _draw():
	if colliderNormal:
#		var angle = rad2deg(colliderNormal.angle_to(Vector2.UP))
		var from = sprite
		draw_line(from.position, from.position + (colliderNormal * 10), Color.red, 1)
