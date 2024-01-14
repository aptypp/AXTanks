extends RigidBody2D

@export var move_speed: float
@export var rotation_speed: float

@export var bullet_position: Node2D
@export var bullet_scene: PackedScene

var move_direction: Vector2

const DELTA_COMPENSATION: float = 100

func _physics_process(delta: float) -> void:
	linear_velocity = transform.y * move_direction.y * move_speed * delta * DELTA_COMPENSATION
	angular_velocity = move_direction.x * rotation_speed * delta * DELTA_COMPENSATION

func _on_input_move_input_changed(new_value: Vector2) -> void:
	move_direction = new_value


func _on_input_shoot_input_triggered() -> void:
	var bullet_instance: Node2D = bullet_scene.instantiate()
	
	bullet_instance.position = bullet_position.global_position
	bullet_instance.rotation = rotation
	
	get_parent().add_child(bullet_instance)
