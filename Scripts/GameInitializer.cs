using Godot;

namespace AXTanks.Scripts;

public partial class GameInitializer : Node
{
    [Export] private Camera2D _camera2D;
    [Export] private MazeGenerator _mazeGenerator;
    
    public override void _Ready()
    {
        _mazeGenerator.Initialize();
        _mazeGenerator.Generate();
        
        _camera2D.Position = _mazeGenerator.size / 2;
    }
}