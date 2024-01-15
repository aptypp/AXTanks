using Godot;

public partial class BulletView : RigidBody2D
{
    [Export] private float _speed;

    public override void _Ready()
    {
        LinearVelocity = -Transform.Y * _speed;
    }

    public override void _Process(double delta)
    {
    }

    private void _on_area_2d_body_entered(RigidBody2D _)
    {
        QueueFree();
    }

    private void _on_timer_timeout()
    {
        QueueFree();
    }
}