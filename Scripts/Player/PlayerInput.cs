using Godot;

namespace AXTanks.Scripts.Player;

public partial class PlayerInput : Node
{
    [Signal]
    public delegate void ShootInputTriggeredEventHandler();

    [Signal]
    public delegate void MoveInputChangedEventHandler(Vector2 input);

    private Vector2 _moveInput;

    public override void _Process(double delta)
    {
        process_move_input();
        process_shoot_input();
    }

    private void process_move_input()
    {
        Vector2 newMoveInput = Vector2.Zero;
        newMoveInput.X = Input.GetAxis("rotate_left", "rotate_right");
        newMoveInput.Y = Input.GetAxis("move_forward", "move_backward");

        if (newMoveInput == _moveInput) return;

        _moveInput = newMoveInput;
        EmitSignal(SignalName.MoveInputChanged, newMoveInput);
    }

    private void process_shoot_input()
    {
        if (!Input.IsActionJustPressed("shoot")) return;

        EmitSignal(SignalName.ShootInputTriggered);
    }
}