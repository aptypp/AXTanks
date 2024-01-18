using System;

namespace AXTanks.Scripts.Player;

public struct TankViewCallbacks
{
    public Action onDead;
    public Func<bool> canShoot;
    public Action<int> decreaseBullet;
    public Action<BulletView> addBullet;
    public Action<BulletView> removeBullet;
}