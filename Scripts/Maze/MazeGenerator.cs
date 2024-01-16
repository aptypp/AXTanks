using System;
using AXTanks.Scripts.Extensions;
using Godot;

namespace AXTanks.Scripts.Maze;

public partial class MazeGenerator : Node2D
{
    public Vector2I size { get; private set; }

    [Export] private Vector2I _halfMinSize;
    [Export] private Vector2I _halfMaxSize;
    [Export] private PackedScene _wallSquareScene;
    [Export] private PackedScene _wallVerticalScene;
    [Export] private PackedScene _wallHorizontalScene;

    private int _seed;
    private Random _random;
    private MazeGeneratorModel _mazeGeneratorModel;

    public void Initialize(int seed)
    {
        _seed = seed;
        _random = new Random(_seed);
        _mazeGeneratorModel = new MazeGeneratorModel(_random);
    }

    public void Generate()
    {
        int sizeX = 2 * _random.Next(_halfMinSize.X, _halfMaxSize.X) + 1;
        int sizeY = 2 * _random.Next(_halfMinSize.Y, _halfMaxSize.Y) + 1;

        size = new Vector2I(sizeX, sizeY);

        MazeElement[,] maze = _mazeGeneratorModel.GenerateMaze(sizeX, sizeY);

        SpawnGrid(maze);
    }

    private void SpawnGrid(MazeElement[,] maze)
    {
        Vector2I pointer = Vector2I.Zero;

        for (int row = 0; row < maze.GetLength(0); row++)
        {
            for (int column = 0; column < maze.GetLength(1); column++)
            {
                if (maze[row, column] == MazeElement.Wall)
                {
                    if (row % 2 == 0)
                    {
                        if (column % 2 == 0)
                        {
                            WallView wallInstance = _wallSquareScene.Instantiate<WallView>();
                            wallInstance.Position = pointer;
                            pointer.X += 32;
                            GetParent().CallDeferredExt(nameof(AddChild), wallInstance);
                        }
                        else
                        {
                            WallView wallInstance = _wallHorizontalScene.Instantiate<WallView>();
                            wallInstance.Position = pointer;
                            pointer.X += 160;
                            GetParent().CallDeferredExt(nameof(AddChild), wallInstance);
                        }
                    }
                    else
                    {
                        if (column % 2 == 0)
                        {
                            WallView wallInstance = _wallVerticalScene.Instantiate<WallView>();
                            wallInstance.Position = pointer;
                            pointer.X += 32;
                            GetParent().CallDeferredExt(nameof(AddChild), wallInstance);
                        }
                    }
                }
                else
                {
                    if (column % 2 == 0)
                    {
                        pointer.X += 32;
                    }
                    else
                    {
                        pointer.X += 160;
                    }
                }
            }

            pointer.X = 0;

            if (row % 2 == 0)
            {
                pointer.Y += 32;
            }
            else
            {
                pointer.Y += 160;
            }
        }
    }
}