using Godot;

namespace AXTanks.Scripts.Maze;

public partial class WallView : StaticBody2D
{
    [Export] public Vector2I size { get; private set; }
}