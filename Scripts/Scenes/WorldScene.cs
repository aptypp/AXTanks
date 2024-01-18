using Godot;

namespace AXTanks.Scripts.Scenes;

public partial class WorldScene : Node2D
{
    [Export] private Maze.MazeGenerator _mazeGenerator;

    public Vector2I GetMazeSize() => _mazeGenerator.size;

    [Rpc(CallLocal = true)]
    public void InitializeMaze(int seed)
    {
        _mazeGenerator.Initialize(seed);
    }

    [Rpc(CallLocal = true)]
    public void GenerateMaze()
    {
        _mazeGenerator.Generate();
    }

    [Rpc(CallLocal = true)]
    public void ClearMaze()
    {
        _mazeGenerator.Clear();
    }
}