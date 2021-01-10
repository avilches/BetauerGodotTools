extends Node

# Tiene que ser Node porque es Autload, y se carga en la escena.
# Tambien es accesible como singleton desde el editor con el nombre PlatformManager

const FALL_PLATFORM_LAYER = 2
const REGULAR_PLATFORM_LAYER = 0

# Configura el layer de la plataforma
func register_platform(platform:Platform):
	print("Platform:", platform.name)
	platform.add_to_group("platform")
	platform.set_collision_layer_bit(REGULAR_PLATFORM_LAYER, true)

func register_falling_platform(platform:KinematicBody2D):
	print("Falling platform:", platform.name)
	for col in platform.get_children():
		if col is CollisionShape2D:
			col.one_way_collision = true
	platform.add_to_group("platform")
	platform.add_to_group("falling_platform")
	platform.set_collision_layer_bit(REGULAR_PLATFORM_LAYER, false)
	platform.set_collision_layer_bit(FALL_PLATFORM_LAYER, true)

func register_moving_platform(platform:KinematicBody2D):
	print("Moving platform:", platform.name)
	platform.set_sync_to_physics(true)
	platform.add_to_group("platform")
	platform.set_collision_layer_bit(REGULAR_PLATFORM_LAYER, true)
	platform.add_to_group("moving_platform")
	
func stop_moving_platform(platform:KinematicBody2D):
	platform.remove_from_group("moving_platform")

func is_platform(platform:KinematicBody2D) -> bool:
	return platform.is_in_group("platform")

func is_moving_platform(platform:KinematicBody2D) -> bool:
	return platform.is_in_group("moving_platform")

func is_falling_platform(platform:KinematicBody2D) -> bool:
	return platform.is_in_group("falling_platform")

# Provoca caida de la plataforma quitando la mascara
func fall_from_platform(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(FALL_PLATFORM_LAYER, false)

func is_falling_from_platform(kb2d:KinematicBody2D):
	return kb2d.get_collision_mask_bit(FALL_PLATFORM_LAYER) == false

# Deja la mascara como estaba (cuando toca el suelo o cuando sale del area2d que ocupa la plataforma)
func stop_falling_from_platform(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(FALL_PLATFORM_LAYER, true)

signal platform_fall_started

# añade una plataforma de la que se puede traspasar, suscribendola (como a todas) a la misma
# señal para que cuando entre en cualquiera de ellas, se pueda volver a activar
func add_area2d_platform_exit(area2D:Area2D):
	area2D.connect("body_shape_entered", self, "_on_Area2D_body_shape_entered")

# se suscribe a la señal de cualquier plataforma de la que se caiga (no importa cual)
func subscribe_platform_out(o, f):
	self.connect("platform_fall_started", o, f)

func _on_Area2D_body_shape_entered(body_id, body, body_shape, area_shape):
	emit_signal("platform_fall_started")
