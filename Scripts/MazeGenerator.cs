using System;
using System.Text;
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
        _size = new Vector2I(2 * Random.Shared.Next(_minSize.X, _maxSize.X) + 1,
            2 * Random.Shared.Next(_minSize.Y, _maxSize.Y) + 1);

        MazeElement[,] maze = MazeGeneratorGpt.GenerateMaze(_size.X, _size.Y);

        SpawnGrid(_size);

        StringBuilder builder = new StringBuilder();

        /*for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                switch (maze[i, j])
                {
                    case MazeElement.Air:
                        builder.Append(" ");
                        break;
                    case MazeElement.Wall:
                        builder.Append("\u2588");
                        break;
                }
            }

            builder.Append('\n');
        }

        GD.Print(builder.ToString());*/
    }

    public override void _Process(double delta)
    {
    }

    private void SpawnGrid(Vector2I size)
    {
        Vector2I offset = Vector2I.Zero;

        for (int column = 0; column < size.X; column++)
        {
            for (int row = 0; row < size.Y; row++)
            {
                PackedScene scene;
                bool isHorizontal = row % 2 == 0;

                if (column % 2 == 0 && row % 2 == 0)
                {
                    scene = _wallSquareScene;
                }
                else if (column > 0 && row > 0 && column < size.X - 1 && row < size.Y - 1)
                {
                    continue;
                }
                else
                {
                    scene = _wallHorizontalScene;
                }

                WallView wallInstance = scene.Instantiate<WallView>();

                wallInstance.Position = new Vector2(isHorizontal ? offset.X - wallInstance.size.X * 1.5f : 0,
                    !isHorizontal ? offset.Y - wallInstance.size.Y * 1.5f : 0);
                offset += wallInstance.size;
                GetParent().CallDeferred("add_child", wallInstance);
            }
        }
    }
}