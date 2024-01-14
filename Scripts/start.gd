extends Node

@export var ui_scene: PackedScene
@export var world_scene: PackedScene

func _ready() -> void:
	var ui_scene_instance: Node = ui_scene.instantiate()
	var world_scene_instance: Node = world_scene.instantiate()
	add_child(ui_scene_instance)
	add_child(world_scene_instance)
	
