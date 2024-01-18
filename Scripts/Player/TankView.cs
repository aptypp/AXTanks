using System;
using AXTanks.Scripts.Extensions;
using Godot;

namespace AXTanks.Scripts.Player;

public partial class TankView : CharacterBody2D
{
    [Export] private float _moveSpeed;
    [Export] private float _rotationSpeed;
    [Export] private Node2D _bulletPosition;
    [Export] private Sprite2D _view;
    [Export] private TankHitBox _hitBox;
    [Export] private PackedScene _bulletScene;

    private Vector2 _moveDirection;
    private Action _onDead;
    private Action<BulletView> _addBullet;
    private Action<BulletView> _removeBullet;
    private bool _isLocalPlayer;

    public void Initialize(bool isLocalPlayer, Color color, Action onDead, Action<BulletView> addBullet,
        Action<BulletView> removeBullet)
    {
        _isLocalPlayer = isLocalPlayer;
        _view.Modulate = color;
        _onDead = onDead;
        _removeBullet = removeBullet;
        _addBullet = addBullet;
    }

    public void SubscribeHitBox() => _hitBox.BodyEntered += OnHitBoxBodyEntered;

    public override void _PhysicsProcess(double delta)
    {
        if (!_isLocalPlayer) return;

        MoveAndSlide();
        Velocity = Transform.Y * _moveDirection.Y * _moveSpeed * (float)delta;
        RotationDegrees += _moveDirection.X * _rotationSpeed * (float)delta;
    }

    public void OnMoveInputChanged(Vector2 direction) => _moveDirection = direction;

    public void OnShootInputTriggered()
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;

        this.RpcServerOnly(nameof(RequestSpawnBullet));
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void Die()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        Hide();
        _onDead();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void Respawn(Vector2 position)
    {
        Position = position;
        RotationDegrees = (float)(Random.Shared.NextDouble() * 360);
        Show();
        ProcessMode = ProcessModeEnum.Inherit;
    }

    [Rpc]
    private void RequestSpawnBullet()
    {
        BulletView bulletView = CreateBullet();

        _addBullet(bulletView);
        bulletView.Initialize(_removeBullet);
        bulletView.StartTimer();

        Rpc(nameof(ResponseSpawnBullet));
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void ResponseSpawnBullet()
    {
        CreateBullet();
    }

    private void OnHitBoxBodyEntered(Node2D body)
    {
        if (body is not BulletView bulletView) return;

        Rpc(nameof(Die));
        _removeBullet(bulletView);
    }

    private BulletView CreateBullet()
    {
        BulletView bulletInstance = _bulletScene.Instantiate<BulletView>();
        
        bulletInstance.Position = _bulletPosition.GlobalPosition;
        bulletInstance.Rotation = Rotation;

        GetParent().AddChild(bulletInstance, true);
        
        bulletInstance.SetMultiplayerAuthority(1);

        return bulletInstance;
    }
}