extends Node


const PLAYER_NAME = "PlayerBody"

signal death

func emit_death(cause = null):
	emit_signal("death", cause)

func isPlayer(kb2d):
	return kb2d.name == PLAYER_NAME
