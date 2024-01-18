using System;

namespace AXTanks.Scripts.Scenes;

public struct GameClientData : IEquatable<GameClientData>
{
    public int id { get; set; }
    public int score { get; set; }
    public int bulletsCount { get; set; }
    public bool isDead { get; set; }

    public bool Equals(GameClientData other) => id == other.id;

    public override bool Equals(object obj) => obj is GameClientData other && Equals(other);

    public override int GetHashCode() => id;
}