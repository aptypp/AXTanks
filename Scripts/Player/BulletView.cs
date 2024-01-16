using Godot;

namespace AXTanks.Scripts.Player;

public partial class BulletView : RigidBody2D
{
    [Export] private float _speed;

    public override void _Ready()
    {
        LinearVelocity = -Transform.Y * _speed;
    }

    private void _on_area_2d_body_entered(RigidBody2D _)
    {
        if (!Multiplayer.IsServer()) return;

        QueueFree();
    }

    private void _on_timer_timeout()
    {
        if (!Multiplayer.IsServer()) return;

        QueueFree();
    }
}