using AXTanks.Scripts.Extensions;
using Godot;

namespace AXTanks.Scripts.Player;

public partial class TankView : CharacterBody2D
{
    public int id => _id;

    [Export] private int _id;
    [Export] private float _moveSpeed;
    [Export] private float _rotationSpeed;
    [Export] private Node2D _bulletPosition;
    [Export] private PackedScene _bulletScene;

    private Vector2 _moveDirection;
    private bool _isLocalPlayer;

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void Initialize(int id)
    {
        _id = id;

        SetMultiplayerAuthority(id);

        _isLocalPlayer = Multiplayer.GetUniqueId() == id;
    }

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
        this.RpcServerOnly(nameof(SpawnBullet));
    }

    [Rpc]
    private void SpawnBullet()
    {
        BulletView bulletInstance = _bulletScene.Instantiate<BulletView>();

        bulletInstance.Position = _bulletPosition.GlobalPosition;
        bulletInstance.Rotation = Rotation;

        GetParent().AddChild(bulletInstance, true);
    }
}