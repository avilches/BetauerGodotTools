extends Node

# Tiene que ser Node porque es Autload, y se carga en la escena.
# Tambien es accesible como singleton desde el editor con el nombre PlatformManager

const REGULAR_PLATFORM_LAYER = 0
const SLOPE_STAIRS_LAYER = 1
const SLOPE_STAIRS_COVER_LAYER = 2
const FALL_PLATFORM_LAYER = 3

const PLAYER_LAYER = 10

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
	platform.set_collision_layer_bit(SLOPE_STAIRS_LAYER, true)

func register_slope_stairs_cover(platform:PhysicsBody2D):
	print("Slope stair cover platform:", platform.name)
	platform.add_to_group("slope_stairs_cover")
	platform.collision_layer = 0
	platform.set_collision_layer_bit(SLOPE_STAIRS_COVER_LAYER, true)

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

func is_a_platform(platform:PhysicsBody2D) -> bool:
	return platform.is_in_group("platform")

func is_a_moving_platform(platform:KinematicBody2D) -> bool:
	return platform.is_in_group("moving_platform")

func is_a_falling_platform(platform:PhysicsBody2D) -> bool:
	return platform.is_in_group("falling_platform")

func is_a_slope_stairs(platform:PhysicsBody2D) -> bool:
	return platform.is_in_group("slope_stairs")

func configure_body(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(REGULAR_PLATFORM_LAYER, true)
	kb2d.set_collision_mask_bit(FALL_PLATFORM_LAYER, true)
	kb2d.set_collision_mask_bit(SLOPE_STAIRS_LAYER, false)
	
# Provoca la caida del jugador desde la plataforma quitando la mascara
func body_fall_from_platform(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(FALL_PLATFORM_LAYER, false)




func body_enable_slope_stairs(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(SLOPE_STAIRS_LAYER, true)

func body_disable_slope_stairs(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(SLOPE_STAIRS_LAYER, false)

func body_has_slope_stairs_enabled(kb2d:KinematicBody2D):
	return kb2d.get_collision_mask_bit(SLOPE_STAIRS_LAYER)




func body_enable_slope_stairs_cover(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(SLOPE_STAIRS_COVER_LAYER, true)

func body_disable_slope_stairs_cover(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(SLOPE_STAIRS_COVER_LAYER, false)

func body_has_slope_stairs_cover_enabled(kb2d:KinematicBody2D):
	return kb2d.get_collision_mask_bit(SLOPE_STAIRS_COVER_LAYER)




# ¿Esta el jugador cayendo de una plataforma?
func is_body_falling_from_platform(kb2d:KinematicBody2D):
	return kb2d.get_collision_mask_bit(FALL_PLATFORM_LAYER) == false

# Deja la mascara como estaba (cuando toca el suelo o cuando sale del area2d que ocupa la plataforma)
func body_stop_falling_from_platform(kb2d:KinematicBody2D):
	kb2d.set_collision_mask_bit(FALL_PLATFORM_LAYER, true)

signal platform_fall_started

# añade un area2D en la que cualquier objeto que la traspase, enviara una señal
# Suscribirse a esta señal desde el jugador para llamar a body_stop_falling_from_platform
func add_area2d_platform_exit(area2D:Area2D):
	area2D.connect("body_shape_entered", self, "_on_Area2D_platform_exit_body_shape_entered")

func _on_Area2D_platform_exit_body_shape_entered(body_id, body, body_shape, area_shape):
	emit_signal("platform_fall_started")

# se suscribe a la señal de cualquier plataforma de la que se caiga (no importa cual)
func subscribe_platform_out(o, f):
	self.connect("platform_fall_started", o, f)



signal slope_stairs_down_in(body, area2D)
signal slope_stairs_down_out(body, area2D)
signal slope_stairs_up_in(body, area2D)
signal slope_stairs_up_out(body, area2D)
signal slope_stairs_enabler_in(body, area2D)
signal slope_stairs_enabler_out(body, area2D)
signal slope_stairs_disabler_in(body, area2D)
signal slope_stairs_disabler_out(body, area2D)

# añade un area2D en la que cualquier objeto que la traspase, enviara una señal
# Suscribirse a esta señal desde el jugador para llamar a body_*
func add_area2d_slope_stairs_down(area2D:Area2D):
	area2D.connect("body_shape_entered", self, "_on_Area2D_slope_stairs_down_body_shape_entered", [area2D])
	area2D.connect("body_shape_exited", self, "_on_Area2D_slope_stairs_down_body_shape_exited", [area2D])

func add_area2d_slope_stairs_up(area2D:Area2D):
	area2D.connect("body_shape_entered", self, "_on_Area2D_slope_stairs_up_body_shape_entered", [area2D])
	area2D.connect("body_shape_exited", self, "_on_Area2D_slope_stairs_up_body_shape_exited", [area2D])

func add_area2d_slope_stairs_enabler(area2D:Area2D):
	area2D.connect("body_shape_entered", self, "_on_Area2D_slope_stairs_enabler_body_shape_entered", [area2D])
	area2D.connect("body_shape_exited", self, "_on_Area2D_slope_stairs_enabler_body_shape_exited", [area2D])

func add_area2d_slope_stairs_disabler(area2D:Area2D):
	area2D.connect("body_shape_entered", self, "_on_Area2D_slope_stairs_disabler_body_shape_entered", [area2D])
	area2D.connect("body_shape_exited", self, "_on_Area2D_slope_stairs_disabler_body_shape_exited", [area2D])

func _on_Area2D_slope_stairs_down_body_shape_entered(body_id, body, body_shape, area_shape, area2D):
	emit_signal("slope_stairs_down_in", body, area2D)

func _on_Area2D_slope_stairs_down_body_shape_exited(body_id, body, body_shape, area_shape, area2D):
	emit_signal("slope_stairs_down_out", body, area2D)

func _on_Area2D_slope_stairs_up_body_shape_entered(body_id, body, body_shape, area_shape, area2D):
	emit_signal("slope_stairs_up_in", body, area2D)

func _on_Area2D_slope_stairs_up_body_shape_exited(body_id, body, body_shape, area_shape, area2D):
	emit_signal("slope_stairs_up_out", body, area2D)

func _on_Area2D_slope_stairs_enabler_body_shape_entered(body_id, body, body_shape, area_shape, area2D):
	emit_signal("slope_stairs_enabler_in", body, area2D)

func _on_Area2D_slope_stairs_enabler_body_shape_exited(body_id, body, body_shape, area_shape, area2D):
	emit_signal("slope_stairs_enabler_out", body, area2D)

func _on_Area2D_slope_stairs_disabler_body_shape_entered(body_id, body, body_shape, area_shape, area2D):
	emit_signal("slope_stairs_disabler_in", body, area2D)

func _on_Area2D_slope_stairs_disabler_body_shape_exited(body_id, body, body_shape, area_shape, area2D):
	emit_signal("slope_stairs_disabler_out", body, area2D)

	
# se suscribe a la señal de cualquier entrada a slope stairs
func subscribe_slope_stairs_down(o, f_in, f_out = null):
	self.connect("slope_stairs_down_in", o, f_in)
	if f_out: self.connect("slope_stairs_down_out", o, f_out)
	
func on_slope_stairs_down_flag(o, flag):
	self.connect("slope_stairs_down_in", self, "_enable_flag", [o, flag])
	self.connect("slope_stairs_down_out", self, "_disable_flag", [o, flag])

func subscribe_slope_stairs_up(o, f_in, f_out = null):
	self.connect("slope_stairs_up_in", o, f_in)
	if f_out: self.connect("slope_stairs_up_out", o, f_out)

func on_slope_stairs_up_flag(o, flag):
	self.connect("slope_stairs_up_in", self, "_enable_flag", [o, flag])
	self.connect("slope_stairs_up_out", self, "_disable_flag", [o, flag])

func subscribe_slope_stairs_enabler(o, f_in, f_out = null):
	self.connect("slope_stairs_enabler_in", o, f_in)
	if f_out: self.connect("slope_stairs_enabler_out", o, f_out)
	
func subscribe_slope_stairs_disabler(o, f_in, f_out = null):
	self.connect("slope_stairs_disabler_in", o, f_in)
	if f_out: self.connect("slope_stairs_disabler_out", o, f_out)
	

func _enable_flag(body, area2d, o, flag):
	if body == o: o[flag] = true

func _disable_flag(body, area2d, o, flag):
	if body == o: o[flag] = false
