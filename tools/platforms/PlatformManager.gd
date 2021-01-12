extends Node

# Tiene que ser Node porque es Autload, y se carga en la escena.
# Tambien es accesible como singleton desde el editor con el nombre PlatformManager

const REGULAR_PLATFORM_LAYER = 0
const SLOPE_STAIRS_LAYER = 1
const FALL_PLATFORM_LAYER = 2

# Configura el layer de la plataforma segun el tipo
func register_platform(platform:PhysicsBody2D):
	print("Platform:", platform.name)
	platform.add_to_group("platform")
	platform.collision_layer = 0
	platform.set_collision_layer_bit(REGULAR_PLATFORM_LAYER, true)

func register_moving_platform(platform:KinematicBody2D):
	print("Moving platform:", platform.name)
	platform.set_sync_to_physics(true)
	platform.add_to_group("platform")
	platform.add_to_group("moving_platform")
	platform.collision_layer = 0
	platform.set_collision_layer_bit(REGULAR_PLATFORM_LAYER, true)

func register_slope_stairs(platform:PhysicsBody2D):
	print("Slope stair platform:", platform.name)
	platform.add_to_group("slope_stairs")
	platform.collision_layer = 0
	platform.set_collision_layer_bit(FALL_PLATFORM_LAYER, true)

func register_falling_platform(platform:PhysicsBody2D):
	print("Falling platform:", platform.name)
	for col in platform.get_children():
		if col is CollisionShape2D:
			col.one_way_collision = true
	platform.add_to_group("platform")
	platform.add_to_group("falling_platform")
	platform.collision_layer = 0
	platform.set_collision_layer_bit(FALL_PLATFORM_LAYER, true)
	
func disable_moving_platform(platform:KinematicBody2D):
	platform.remove_from_group("moving_platform")

func enable_moving_platform(platform:KinematicBody2D):
	platform.remove_from_group("moving_platform")

func is_platform(platform:PhysicsBody2D) -> bool:
	return platform.is_in_group("platform")

func is_moving_platform(platform:KinematicBody2D) -> bool:
	return platform.is_in_group("moving_platform")

func is_falling_platform(platform:PhysicsBody2D) -> bool:
	return platform.is_in_group("falling_platform")

func is_slope_stairs(platform:PhysicsBody2D) -> bool:
	return platform.is_in_group("slope_stairs")

# Provoca la caida del jugador desde la plataforma quitando la mascara
func fall_from_platform(kb2d:PhysicsBody2D):
	kb2d.set_collision_mask_bit(FALL_PLATFORM_LAYER, false)

# ¿Esta el jugador cayendo de una plataforma?
func is_falling_from_platform(kb2d:KinematicBody2D):
	return kb2d.get_collision_mask_bit(FALL_PLATFORM_LAYER) == false

# Deja la mascara como estaba (cuando toca el suelo o cuando sale del area2d que ocupa la plataforma)
func stop_falling_from_platform(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(FALL_PLATFORM_LAYER, true)

signal platform_fall_started

# añade un area2D en la que cualquier objeto que la traspase, enviara una señal
# Suscribirse a esta señal desde el jugador para llamar a stop_falling_from_platform
func add_area2d_platform_exit(area2D:Area2D):
	area2D.connect("body_shape_entered", self, "_on_Area2D_body_shape_entered")

# se suscribe a la señal de cualquier plataforma de la que se caiga (no importa cual)
func subscribe_platform_out(o, f):
	self.connect("platform_fall_started", o, f)

func _on_Area2D_body_shape_entered(body_id, body, body_shape, area_shape):
	emit_signal("platform_fall_started")


