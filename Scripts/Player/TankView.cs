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
    private TankViewCallbacks _callbacks;

    private bool _isLocalPlayer;

    public void Initialize(bool isLocalPlayer, Color color, TankViewCallbacks callbacks)
    {
        _isLocalPlayer = isLocalPlayer;
        _view.Modulate = color;
        _callbacks = callbacks;
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

        this.RpcServerOnly(nameof(RequestSpawnBullet), Multiplayer.GetUniqueId(), _view.Modulate);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void Die()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        Hide();
        _callbacks.onDead();
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
    private void RequestSpawnBullet(int id, Color color)
    {
        if (!_callbacks.canShoot()) return;

        BulletView bulletView = CreateBullet(color);

        _callbacks.addBullet(bulletView);
        _callbacks.decreaseBullet(id);
        bulletView.Initialize(_callbacks.removeBullet, id);
        bulletView.StartTimer();

        Rpc(nameof(ResponseSpawnBullet), color);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void ResponseSpawnBullet(Color color)
    {
        CreateBullet(color);
    }

    private void OnHitBoxBodyEntered(Node2D body)
    {
        if (body is not BulletView bulletView) return;

        CallDeferred(nameof(TankHit), bulletView);
    }

    private void TankHit(BulletView bulletView)
    {
        Rpc(nameof(Die));
        _callbacks.removeBullet(bulletView);
    }

    private BulletView CreateBullet(Color color)
    {
        BulletView bulletInstance = _bulletScene.Instantiate<BulletView>();

        bulletInstance.SetColor(color);

        bulletInstance.Position = _bulletPosition.GlobalPosition;
        bulletInstance.Rotation = Rotation;

        GetParent().AddChild(bulletInstance, true);

        bulletInstance.SetMultiplayerAuthority(1);

        return bulletInstance;
    }
}