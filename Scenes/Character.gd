extends KinematicBody2D
class_name Character

var motion: Vector2 = Vector2.ZERO

# Updated manually before move_and.. by update_ground_colliders()
var is_on_slope: bool
var is_on_moving_platform: bool
var is_on_falling_platform: bool
var is_on_slope_stairs: bool
# Changed by signals
var is_on_slope_stairs_down: bool = false
var is_on_slope_stairs_up: bool = false

# Variables only to debug
var colliderNormal: Vector2
var movStartPosMAXSPEED
var lastMotion: Vector2 = Vector2.ZERO
var movStartTimeMAXSPEED = 0
var movStartTimeACC = -1

var C

func configure_falling_platforms():
	PlatformManager.SubscribeFallingPlatformOut(self, "stop_falling_from_platform")
	
	# not falling by default
	stop_falling_from_platform()

func configure_slope_stairs():
	PlatformManager.on_slope_stairs_down_flag(self, "is_on_slope_stairs_down")
	PlatformManager.on_slope_stairs_up_flag(self, "is_on_slope_stairs_up")
	#PlatformManager.SubscribeSlopeStairsUp(self, "_OnSlopeStairsUpIn", "_OnSlopeStairsUpOut")
	#PlatformManager.SubscribeSlopeStairsDown(self, "_OnSlopeStairsDownIn", "_OnSlopeStairsDownOut")

	PlatformManager.SubscribeSlopeStairsEnabler(self, "_OnSlopeStairsEnablerIn") #, "_slope_stairs_enabler_out")
	PlatformManager.SubscribeSlopeStairsDisabler(self, "_OnSlopeStairsDisablerIn") #, "_slope_stairs_disabler_out")

	# disabled by default
	DisableSlopeStairs()

#func slope_stairs_up_in(body, _area2D):
#	if body == self:
#		print("Entering up hall...")
#
#func slope_stairs_up_out(body, _area2D):
#	if body == self:
#		print("Exiting up hall...")
#
#func _OnSlopeStairsUpIn(body, area2D):
#	if body == self: print("stairs_up_in")
#
#func _OnSlopeStairsUpOut(body, area2D):
#	if body == self: print("stairs_up_out")
#
#func _OnSlopeStairsDownIn(body, area2D):
#	if body == self: print("stairs_down_in")
#
#func _OnSlopeStairsDownOut(body, area2D):
#	if body == self: print("stairs_down_out")

func _OnSlopeStairsEnablerIn(body, _area2D):
	if body == self:
		EnableSlopeStairs()

#func _slope_stairs_enabler_out(body, _area2D):
#	if body == self:
#		print("stairs_enabler_out")

func _OnSlopeStairsDisablerIn(body, _area2D):
	if body == self:
		DisableSlopeStairs()

#func _slope_stairs_disabler_out(body, _area2D):
#	if body == self: 
#		print("stairs_disabler_out")

func EnableSlopeStairs():
	# permite subir una escalera
	if C.DEBUG_SLOPE_STAIRS: print("stairs_enabler_in ENABLING")
	PlatformManager.DisableSlopeStairsCoverForBody(self)
	PlatformManager.EnableSlopeStairsForBody(self)
	
func DisableSlopeStairs():
	# deja de subir la escalera
	if C.DEBUG_SLOPE_STAIRS: print("stairs_disabler_in DISABLING")
	PlatformManager.EnableSlopeStairsCoverForBody(self)
	PlatformManager.DisableSlopeStairsForBody(self)

func fall_from_platform():
	if is_on_falling_platform:
		PlatformManager.BodyFallFromPlatform(self)
		debug_player_masks()

func stop_falling_from_platform():
	PlatformManager.BodyStopFallFromPlatform(self)
	debug_player_masks()

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
		
		# this is like a callback: if the objet has method collide_with, it's called
		if collision.collider.has_method("collide_with"):
			collision.collider.collide_with(self, collision)

		if abs(collision.normal.y) < 1:
			is_on_slope = true
		
		if collision.collider is KinematicBody2D && PlatformManager.IsMovingPlatform(collision.collider):
			is_on_moving_platform = true
		
		if collision.collider is PhysicsBody2D && PlatformManager.IsFallingPlatform(collision.collider):
			is_on_falling_platform = true

		if collision.collider is PhysicsBody2D && PlatformManager.IsSlopeStairs(collision.collider):
			is_on_slope_stairs = true
	update() # this allow to call to _draw() with the colliderNormal updated
	
	
func add_lateral_movement(x_input, delta):
	if x_input != 0:
		if sign(motion.x) != sign(x_input): # si cambia de direccion de repente...
			# esto hace que se puede cambiar de direccion sin esperar a que decelere
			motion.x = x_input * C.ACCELERATION * delta
		else:
			motion.x += x_input * C.ACCELERATION * delta
	else:
		# para suavamente
		if abs(motion.x) < C.STOP_IF_SPEED_IS_LESS_THAN: motion.x = 0
		else: motion.x *= C.FRICTION if is_on_floor() else C.AIR_RESISTANCE

func limit_motion(delta, applyGravity = true, maxSpeedFactor = 1):
	if applyGravity:
		motion.y += C.GRAVITY * delta

	var realMaxSpeed = C.MAX_SPEED * maxSpeedFactor
	motion.x = clamp(motion.x, -realMaxSpeed, realMaxSpeed)
	motion.y = min(motion.y, C.MAX_FALLING_SPEED) # avoid gravity continue forever in free fall

###########################################################################
# DEBUG
###########################################################################

func debug_motion(delta):
	if C.DEBUG_ACCELERATION && motion.x != 0:
		if lastMotion.x == 0:
			movStartTimeACC = 0 # starts to move
		elif movStartTimeACC != -1:
			movStartTimeACC += delta
			if abs(motion.x) >= C.MAX_SPEED:
				print("Full throtle ", motion.x, " in ", movStartTimeACC, "s")
				movStartTimeACC = -1

	if C.DEBUG_MAX_SPEED:
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

	if C.DEBUG_MOTION && (lastMotion.x != motion.x || lastMotion.y != motion.y): print(motion, motion-lastMotion)

func debug_player_masks():
	if C.DEBUG_COLLISION:
		print("Player:  ",int(get_collision_mask_bit(0)), int(get_collision_mask_bit(1)), int(get_collision_mask_bit(2)))

func debug_collision():
	if C.DEBUG_COLLISION && get_slide_count():
		debug_player_masks()
		for i in get_slide_count():
			var collision = get_slide_collision(i)
			print("Collider:",int(collision.collider.get_collision_layer_bit(0)), int(collision.collider.get_collision_layer_bit(1)), int(collision.collider.get_collision_layer_bit(2)), " ", collision.collider.get_class(), ":'", collision.collider.name+"'")

