extends FallMovingPlatform

var original
var follow = Vector2.ZERO

func _enter_tree():
	original = position

func _ready():
	var original = position
	var seq := TweenSequence.new(get_tree())
	seq.append(self, "follow", Vector2(100, 0), 2).set_trans(Tween.TRANS_CUBIC)
	seq.append(self, "follow", Vector2.ZERO, 2).set_trans(Tween.TRANS_CUBIC)
	seq.set_loops()
	
func _physics_process(delta):
	position = original + follow
