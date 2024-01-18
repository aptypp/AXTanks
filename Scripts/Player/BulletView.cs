using System;
using Godot;

namespace AXTanks.Scripts.Player;

public partial class BulletView : RigidBody2D
{
    public int linkedPlayerId => _linkedPlayerId;

    [Export] private float _speed;
    [Export] private Timer _timer;
    [Export] private Sprite2D _view;

    private int _linkedPlayerId;
    private Action<BulletView> _removeBullet;

    public override void _Ready()
    {
        LinearVelocity = -Transform.Y * _speed;
    }

    public void Initialize(Action<BulletView> removeBullet, int playerId)
    {
        _removeBullet = removeBullet;
        _linkedPlayerId = playerId;
    }

    public void SetColor(Color color)
    {
        _view.Modulate = color;
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
        _timer.Stop();
        QueueFree();
    }
}