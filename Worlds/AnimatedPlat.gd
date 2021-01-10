extends FallMovingPlatform

func _ready():
	var original = position
	var seq := TweenSequence.new(get_tree())
	seq.append(self, "position", original + Vector2(100, 0), 2).set_trans(Tween.EASE_IN_OUT)
	seq.append(self, "position", original, 2).set_trans(Tween.EASE_IN_OUT)
	seq.set_loops()
	
	seq.reset()
