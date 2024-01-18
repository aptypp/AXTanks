using System;
using Godot;

namespace AXTanks.Scripts.Player;

public partial class BulletView : RigidBody2D
{
    [Export] private float _speed;
    [Export] private Timer _timer;

    private Action<BulletView> _removeBullet;

    public override void _Ready()
    {
        LinearVelocity = -Transform.Y * _speed;
    }

    public void Initialize(Action<BulletView> removeBullet)
    {
        _removeBullet = removeBullet;
    }

    public void StartTimer()
    {
        _timer.Timeout += OnTimerTimeout;
        _timer.Start();
    }

    private void OnTimerTimeout()
    {
        _removeBullet(this);
    }

    [Rpc(CallLocal = true)]
    public void Destroy()
    {
        QueueFree();
    }
}