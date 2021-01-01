extends Node

# Tiene que ser Node porque es Autload, y se carga en la escena.
# Tambien es accesible como singleton desde el editor con el nombre PlatformManager

const FALL_PLATFORM_LAYER = 2

# Configura el layer de la plataforma (se puede hacer desde el editor)
func add_kineticbody2d_platform(kb2d:KinematicBody2D):
	kb2d.set_collision_layer_bit(FALL_PLATFORM_LAYER, true)

# Provoca caida de la plataforma quitando la mascara
func fall_from_platform(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(FALL_PLATFORM_LAYER, false)

func is_falling_from_platform(kb2d:KinematicBody2D):
	return kb2d.get_collision_mask_bit(FALL_PLATFORM_LAYER) == false

# Deja la mascara como estaba (cuando toca el suelo o cuando sale del area2d que ocupa la plataforma)
func enable_platform_collide(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(FALL_PLATFORM_LAYER, true)

signal platform_fall_started

func add_area2d_platform_exit(area2D:Area2D):
	area2D.connect("body_shape_entered", self, "_on_Area2D_body_shape_entered")

func subscribe_platform_out(o, f):
	self.connect("platform_fall_started", o, f)

func _on_Area2D_body_shape_entered(body_id, body, body_shape, area_shape):
	emit_signal("platform_fall_started")
	pass # Replace with function body.

#func get_all_platforms():
	
