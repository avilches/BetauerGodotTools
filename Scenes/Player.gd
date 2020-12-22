extends KinematicBody2D

# TODO: salto coyote (permitir saltar cuando se acaba de caer)
# TODO: permitir pulsar el salto 0.2 segundos antes de llegar al suelo  
# TODO: efectos

const ACCELERATION = 3
const MAX_SPEED = 80
const MAX_FALLING_SPEED = 500
const FRICTION = 9
const AIR_RESISTANCE = 0
#const GRAVITY = 920
#const JUMP_FORCE = 368
var jumpHeight:float = 52;
var timeToJumpApex = 0.4;

onready var GRAVITY = (2 * jumpHeight) / pow(timeToJumpApex, 2)
onready var JUMP_FORCE = GRAVITY * timeToJumpApex;

onready var sprite = $Sprite
onready var animationPlayer = $AnimationPlayer

var motion = Vector2.ZERO

func _physics_process(delta):
	# delta is 1/60 = 0.01666 aprox
	# x * delta -> x / 60, move_and_slide no debe usar tiempos modificados con delta

	var x_input:float = Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left");
	
	if x_input != 0:
		# si cambia de direccion de repente...
		var ACC = ACCELERATION
		if sign(motion.x) != sign(x_input):
			# esto hace que se puede cambiar de direccion sin esperar a que decelere
			motion.x = x_input * ACC
		else:
			motion.x += x_input * ACC

		if abs(motion.x) > MAX_SPEED:
			# test para saber cuando llega a la maxima velocidad
			animationPlayer.play("Run")
			
		sprite.flip_h = x_input < 0
	else:
		# para suavamente
		animationPlayer.play("Stand")
		if is_on_floor():
			#print(FRICTION * delta, ' - ', FRICTION / 60.0)
			motion.x = lerp(motion.x, 0, FRICTION * delta)
		else:
			motion.x = lerp(motion.x, 0, AIR_RESISTANCE * delta)
		
	if is_on_floor():
		if Input.is_action_just_pressed("ui_up"):
			motion.y = -JUMP_FORCE
	else:
		animationPlayer.play("Jump")
		if Input.is_action_just_released("ui_up") and motion.y < -JUMP_FORCE/2:
			motion.y = -JUMP_FORCE/2

	motion.y += (GRAVITY / 60)
	motion.x = clamp(motion.x, -MAX_SPEED, MAX_SPEED)
	#motion.y = max(motion.y, MAX_FALLING_SPEED) # avoid gravity continue forever in free fall
	motion = move_and_slide(motion, Vector2.UP)
	
