using Godot;

namespace AXTanks.Scripts;

public partial class TankView : RigidBody2D
{
    [Export] private float _moveSpeed;
    [Export] private float _rotationSpeed;
    [Export] private Node2D _bulletPosition;
    [Export] private PackedScene _bulletScene;

    private Vector2 _moveDirection;

    public override void _PhysicsProcess(double delta)
    {
        LinearVelocity = Transform.Y * _moveDirection.Y * _moveSpeed * (float)delta;
        AngularVelocity = _moveDirection.X * _rotationSpeed * (float)delta;
    }

    private void _on_view_move_input_changed(Vector2 direction) => _moveDirection = direction;

    private void _on_view_shoot_input_triggered()
    {
        Node2D bulletInstance = _bulletScene.Instantiate<Node2D>();

        bulletInstance.Position = _bulletPosition.GlobalPosition;
        bulletInstance.Rotation = Rotation;

        GetParent().AddChild(bulletInstance);
    }
}