using System;
using Godot;

namespace AXTanks.Scripts.Scenes;

public struct LobbyClientData : IEquatable<LobbyClientData>
{
    public int id { get; set; }
    public bool isReady { get; set; }
    public Color color { get; set; }
    public string name { get; set; }

    public bool Equals(LobbyClientData other) => id == other.id;

    public override bool Equals(object obj) => obj is LobbyClientData other && Equals(other);

    public override int GetHashCode() => id;
}