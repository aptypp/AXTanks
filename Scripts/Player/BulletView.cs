using System;
using Godot;

namespace AXTanks.Scripts.Player;

public partial class BulletView : RigidBody2D
{
    public event Action<TankView> HitTank;

    [Export] private float _speed;

    private Action<BulletView> _removeBullet;

    public override void _Ready()
    {
        LinearVelocity = -Transform.Y * _speed;
    }

    public void Initialize(Action<BulletView> removeBullet)
    {
        _removeBullet = removeBullet;
    }

    private void _on_area_2d_body_entered(CharacterBody2D characterBody2D)
    {
        if (!Multiplayer.IsServer()) return;

        HitTank?.Invoke(characterBody2D as TankView);
        _removeBullet(this);
    }

    private void _on_timer_timeout()
    {
        if (!Multiplayer.IsServer()) return;

        _removeBullet(this);
    }

    [Rpc(CallLocal = true)]
    public void Destroy()
    {
        QueueFree();
    }
}