extends Area2D


export(String, FILE, "*.tscn") var next_scene;

func _physics_process(delta):
	var bodies = get_overlapping_bodies();
	for body in bodies:
		if body.name == "Player":
			# var scene_name = get_tree().get_current_scene().get_name()
			get_tree().change_scene(next_scene)

			
