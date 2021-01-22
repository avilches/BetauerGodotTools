extends KinematicBody2D


const SPEED = 40
const INITIAL_DIRECTION = Vector2.RIGHT

enum State { Walk, Idle }

# TODO: esta constante esta repetida en Player, unificar
const FLOOR = Vector2.UP

var timeInState: float = 0.0
var currentState = State.Idle
var currentDirection = INITIAL_DIRECTION

onready var sprite = $Sprite

func _ready():
	randomize()
	pass


func flip(right):
	sprite.flip_h = !right;

func _physics_process(delta):
	if currentState == State.Idle:
		if timeInState >= 0:
			timeInState -= delta
		else:
			currentState = State.Walk
			timeInState = 1.5
			
	elif currentState == State.Walk:
		if timeInState >= 0:
			timeInState -= delta
			move()
		else:
			currentState = State.Idle
			timeInState = 0.5

	
	
func move():
	move_and_slide(currentDirection * SPEED, Vector2.UP)
	for i in get_slide_count():
		var collision = get_slide_collision(i)
		if abs(collision.normal.x) == 1:
			# colision lateral
			currentDirection *= Vector2(-1, 1)
		
	flip(currentDirection.x == 1)
	
	
	
	
