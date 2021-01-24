extends Character

# TODO: efectos
# escalar a las paredes
# engancharse en las repisas de las plataformas

onready var sprite = $Sprite
onready var anim_sprite = $Sprite/AnimationPlayer

var jumps = 0
var canJump = false
var isJumping = false
var time_jump_pressed = -1

enum Anim { JUMP, FALL, IDLE, RUN }

func change_sprite(next: int):
	match next:
		Anim.JUMP: anim_sprite.play("Jump")
		Anim.FALL: anim_sprite.play("Fall")
		Anim.RUN: anim_sprite.play("Run")
		Anim.IDLE, _: anim_sprite.play("Idle")
			

func update_sprite(_delta, x_input):
	if isJumping:
		if motion.y > C.START_FALLING_SPEED:
			change_sprite(Anim.FALL)
		else:
			change_sprite(Anim.JUMP)
	elif x_input != 0:
		change_sprite(Anim.RUN)
		
	else:
		if motion.y > 0:
			if motion.y > C.START_FALLING_SPEED:
				change_sprite(Anim.FALL)
			else:
				change_sprite(Anim.RUN)
		else:
			change_sprite(Anim.IDLE)

	if x_input != 0:
		flip(x_input < 0)


func _ready():
	C = PlayerConfig.new()
	if !GameManager.isPlayer(self):
		var msg = "Player node name should be "+GameManager.PLAYER_NAME+" (current name: "+self.name+")"
		print(msg)
		push_error(msg)

	configure_falling_platforms()
	configure_slope_stairs()
	
	GameManager.connect("death", self, "on_death")


func on_death(_cause):
	print("MUETO")
	set_process(false)
	set_physics_process(false)
	#Engine.set_target_fps(30)
	

func jump(delta):
	if is_on_floor():
		jumps = 0
		canJump = true
		
	if Input.is_action_just_pressed("ui_select"):
		# jump starts
		time_jump_pressed = 0
	elif time_jump_pressed != -1:
		# is jumping
		time_jump_pressed += delta
		
	if canJump:
		if time_jump_pressed != -1 && time_jump_pressed <= C.JUMP_HELPER_TIME:
			if C.DEBUG_JUMP:
				if (time_jump_pressed > 0): print("Jump helper: ", time_jump_pressed, "s (Config:", C.JUMP_HELPER_TIME,")")
				print("Jump start:", -C.JUMP_FORCE)
			time_jump_pressed = -1
			if C.SQUEEZE_JUMP_TIME != 0: sprite.scale = C.SQUEEZE_JUMP_SCALE
			
			motion.y = -C.JUMP_FORCE
			jumps = jumps + 1
			isJumping = true
			canJump = jumps < C.MAX_JUMPS
			# TODO: controlar que el segundo salto se haga justo arriba, no al llegar al suelo
			#       para que no coincida con el helper...
	
	
	if Input.is_action_just_released("ui_select") && motion.y < -C.JUMP_FORCE_MIN && isJumping:
		if C.DEBUG_JUMP:
			print("Short jump: deccelerating from ", motion.y, " to ", -C.JUMP_FORCE_MIN)
		motion.y = -C.JUMP_FORCE_MIN
	


var applyGravity = true


func _physics_process(delta):
	
#	$Label.text = str(PlatformManager.body_has_slope_stairs_enabled(self), " ", PlatformManager.body_has_slope_stairs_cover_enabled(self))
#	$Label.text = str(is_on_slope_stairs_down, " ", is_on_slope_stairs_up)

	var x_input = Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left")
	add_lateral_movement(x_input, delta)
	jump(delta)

	if applyGravity:
		motion.y += C.GRAVITY * delta

	var realMaxSpeed = C.MAX_SPEED * 1.5 if Input.is_action_pressed("ui_accept") else C.MAX_SPEED
	motion.x = clamp(motion.x, -realMaxSpeed, realMaxSpeed)
	motion.y = min(motion.y, C.MAX_FALLING_SPEED) # avoid gravity continue forever in free fall

	debug_motion(delta)

	var slowdownVector = Vector2.ONE
	var slope_down = null
	
	if is_on_slope && !isJumping && x_input != 0:
		slope_down = sign(colliderNormal.x) == sign(x_input) # pendiente y direccion al mismo lado
		slowdownVector = C.SLOW_ON_SLOPE_DOWN if slope_down else C.SLOW_ON_SLOPE_UP
	
	if is_on_slope_stairs_down:
		if Input.is_action_pressed("ui_up") && !isJumping:
			enable_slope_stairs()
		else: #if x_input != 0:
			disable_slope_stairs()
	elif is_on_slope_stairs_up:
		if Input.is_action_pressed("ui_down") && !isJumping && (!colliderNormal || abs(colliderNormal.x) != 1):
			# Hay un bug en el que si se sube la escaleras dejando pulsado abajo,
			# cuando llega arriba y la pendiente finaliza, colisiona lateralmente
			# con la plataforma. Esta collision es (1, 0) subiendo de derecha a 
			# izquierda (es decir, completamente lateral) y el personaje se queda
			# atascado y no llega a subir. Tambien pasa desde arriba, es decir, 
			# andando de izquierda a derecha pulsando abajo.
			# Claro que dejando de pulsar abajo se
			# soluciona, pero no deja de ser un bug. Lo ideal sería controlar
			# la dirección de la escalera y activarla solo cuando se avanza hacia ella
			# pero con el fix abs(colliderNormal.x) != 1) se arregla sin tener que
			# hacerlo. 
			enable_slope_stairs()
		elif x_input != 0:
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
		var remain = move_and_slide_with_snap(motion * slowdownVector, Vector2.ZERO, C.FLOOR, STOP_ON_SLOPES)
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
		var remain = move_and_slide_with_snap(motion * slowdownVector, C.SLOPE_RAYCAST_VECTOR, C.FLOOR, STOP_ON_SLOPES)
		motion.y = remain.y  # this line stops the gravity accumulation
#		motion.x = remain.x  # this line should be always commented, player can't climb slopes with it!!
		
	update_ground_colliders()
	
	if !was_in_floor && is_on_floor():
		debug_collision()
		# just grounded
		if C.SQUEEZE_LAND_TIME != 0: sprite.scale = C.SQUEEZE_LAND_SCALE
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

func schedule_coyote_time():
	if C.COYOTE_TIME > 0:
		yield(get_tree().create_timer(C.COYOTE_TIME), "timeout")
	canJump = false

func restore_squeeze():
	if is_on_floor():
		if C.SQUEEZE_LAND_TIME != 0:
			sprite.scale.y = lerp(sprite.scale.y, 1, C.SQUEEZE_LAND_TIME)
			sprite.scale.x = lerp(sprite.scale.x, 1, C.SQUEEZE_LAND_TIME)
	else:
		if C.SQUEEZE_JUMP_TIME != 0:
			sprite.scale.y = lerp(sprite.scale.y, 1, C.SQUEEZE_JUMP_TIME)
			sprite.scale.x = lerp(sprite.scale.x, 1, C.SQUEEZE_JUMP_TIME)

func flip(left):
	sprite.flip_h = left;



func _draw():
	if colliderNormal:
#		var angle = rad2deg(colliderNormal.angle_to(Vector2.UP))
		# Se pinta una linea hacia arriba desde la posicion 0,0 del sprite
		# Para que no la pinte desde el centro, sino desde abajo, hay que desplazar
		# el sprite con offset dejando el position a 0,0
		var from = sprite
		draw_line(from.position, from.position + (colliderNormal * 10), Color.red, 1)

