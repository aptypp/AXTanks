using Godot;

namespace AXTanks.Scripts.Player;

public partial class TankHitBox : Area2D
{
    [Export] public TankView tankView { get; set; }
}