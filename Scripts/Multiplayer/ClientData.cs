using System;
using Godot;

namespace AXTanks.Scripts.Scenes;

public struct ClientData : IEquatable<ClientData>
{
    public int id { get; set; }
    public bool isReady { get; set; }
    public Color color { get; set; }
    public string name { get; set; }

    public bool Equals(ClientData other) => id == other.id;

    public override bool Equals(object obj) => obj is ClientData other && Equals(other);

    public override int GetHashCode() => id;
}