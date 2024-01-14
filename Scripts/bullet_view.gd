extends RigidBody2D

@export var speed: float 

func _ready() -> void:
	linear_velocity = -transform.y * speed


func _on_area_2d_body_entered(_body: RigidBody2D) -> void:
	queue_free()
