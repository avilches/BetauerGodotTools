extends TileMap

export(PackedScene) var tree_scene

func spawn_player_in(tile_position: Vector2):
	var tree = tree_scene.instance()
	var world_position = map_to_world(tile_position)
	print("Adding a player instance from tile ", tile_position, " to ", world_position)
	tree.position = world_position 
	set_cellv(tile_position, -1)
	# bug!! https://github.com/godotengine/godot/issues/233
	get_parent().call_deferred("add_child", tree)
#	get_parent().add_child(tree)

func transform_tiles() -> void:

	
	# how to find the tile name by id, or get the id by tile name	
	for tile_id in tile_set.get_tiles_ids():
		var tile_name = tile_set.tile_get_name(tile_id)
		print(tile_id, " -> ", tile_name)
		print(name, " <- ", tile_set.find_tile_by_name(tile_name))
	
	# iterate tiles to convert them
	for tile_position in get_used_cells_by_id(tile_set.find_tile_by_name("Player")):
		var tile = get_cellv(tile_position)
		spawn_player_in(tile_position)

func _ready():
	transform_tiles()
