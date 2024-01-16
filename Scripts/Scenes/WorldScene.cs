using AXTanks.Scripts.Player;
using Godot;

namespace AXTanks.Scripts.Scenes;

public partial class WorldScene : Node2D
{
    [Export] private Maze.MazeGenerator _mazeGenerator;

    public void Initialize(PlayerInput playerInput)
    {
        
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void GenerateMaze(int seed)
    {
        _mazeGenerator.Initialize(seed);
        _mazeGenerator.Generate();
    }
}