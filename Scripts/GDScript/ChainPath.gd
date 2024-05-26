@tool
extends Path3D

@export var distance_between_links: float = 0.29:
	set(value):
		distance_between_links = value
		spawn_link()
	get:
		return distance_between_links
@export var chain_link: PackedScene

func spawn_link() -> void:
	var path_length: float = curve.get_baked_length()
	var count: int = floor(path_length / distance_between_links)
	var offset = distance_between_links / 2
	
	for child in get_children():
		child.free()
	
	for i in range(count):
		var curve_distance = offset + distance_between_links * i
		var pos = curve.sample_baked(curve_distance, true)
		var forward = pos.direction_to(curve.sample_baked(curve_distance + 0.1, true))
		var up = curve.sample_baked_up_vector(curve_distance, true)
		
		if i % 2 == 0:
			up = up.rotated(forward, deg_to_rad(90))
		
		var link: Node3D = chain_link.instantiate() as Node3D
		add_child(link)
		link.position = pos
		
		var bas = Basis()
		bas.y = up
		bas.z = -forward
		bas.x = forward.cross(up).normalized()
		link.basis = bas

func _on_curve_changed() -> void:
	spawn_link()
