using AXTanks.Scripts.Extensions;
using Godot;

namespace AXTanks.Scripts;

public partial class MazeGenerator : Node2D
{
    [Export] private PackedScene _wallSquareScene;
    [Export] private PackedScene _wallVerticalScene;
    [Export] private PackedScene _wallHorizontalScene;

    [Export] private Vector2I _minSize;
    [Export] private Vector2I _maxSize;

    private Vector2I _size;

    public override void _Ready()
    {
        MazeElement[,] maze = MazeGeneratorGpt.GenerateMaze(_maxSize.X, _maxSize.Y);

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
                            pointer.X += 8;
                            GetParent().CallDeferredExt(nameof(AddChild), wallInstance);
                        }
                        else
                        {
                            WallView wallInstance = _wallHorizontalScene.Instantiate<WallView>();
                            wallInstance.Position = pointer;
                            pointer.X += 40;
                            GetParent().CallDeferredExt(nameof(AddChild), wallInstance);
                        }
                    }
                    else
                    {
                        if (column % 2 == 0)
                        {
                            WallView wallInstance = _wallVerticalScene.Instantiate<WallView>();
                            wallInstance.Position = pointer;
                            pointer.X += 8;
                            GetParent().CallDeferredExt(nameof(AddChild), wallInstance);
                        }
                    }
                }
                else
                {
                    if (column % 2 == 0)
                    {
                        pointer.X += 8;
                    }
                    else
                    {
                        pointer.X += 40;
                    }
                }
            }

            pointer.X = 0;

            if (row % 2 == 0)
            {
                pointer.Y += 8;
            }
            else
            {
                pointer.Y += 40;
            }
        }
    }
}