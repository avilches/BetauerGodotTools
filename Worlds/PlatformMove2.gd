extends KinematicBody2D

onready var path = $'Platform path/PathFollow2D'

onready var original = position
var enabled = false
# Called when the node enters the scene tree for the first time.
func _ready():
	original = position
	print(name, ' layer:', get_collision_layer_bit(0))
	print(name, ' layer:',  get_collision_layer_bit(1))
	print(name, ' layer:', get_collision_layer_bit(2))
	print(name, ' mask:', get_collision_mask_bit(0))
	print(name, ' mask:', get_collision_mask_bit(1))
	print(name, ' mask:', get_collision_mask_bit(2))
	
	
var move_speed = 10
func _physics_process(delta):
	if (enabled):
		position = original + path.position
		path.set_offset(path.get_offset() + 10 * delta)
#	transform.origin = path.transform.origin
	
