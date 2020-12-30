extends KinematicBody2D

var move_speed = 100
export (NodePath) var patrol_path
var patrol_points
var patrol_index = 0
var velocity

func _ready():
	if patrol_path:
		patrol_points = get_node(patrol_path).curve.get_baked_points()
		

func _physics_process(delta):
	if !patrol_path:
		return
	var target = patrol_points[patrol_index]
	if position.distance_to(target) < 1:
		patrol_index = wrapi(patrol_index + 1, 0, patrol_points.size())
		target = patrol_points[patrol_index]
	velocity = (target - position).normalized() * move_speed
	velocity = move_and_slide(velocity)
