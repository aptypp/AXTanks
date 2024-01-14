extends Node

signal move_input_changed(new_value: Vector2)
signal shoot_input_triggered()

var move_input: Vector2

func _process(_delta: float) -> void:
	process_move_input()
	process_shoot_input()
	
func process_move_input() -> void:
	var new_move_input : Vector2 = Vector2.ZERO
	
	new_move_input.x = Input.get_axis("rotate_left", "rotate_right")
	new_move_input.y = Input.get_axis("move_forward", "move_backward")
	
	if new_move_input == move_input:
		return
	
	move_input = new_move_input
	move_input_changed.emit(new_move_input)

func process_shoot_input() -> void:
	if not Input.is_action_just_pressed("shoot"):
		return
		
	shoot_input_triggered.emit()
