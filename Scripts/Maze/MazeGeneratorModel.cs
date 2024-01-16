using System;
using System.Collections.Generic;
using Godot;

namespace AXTanks.Scripts.Maze;

internal class MazeGeneratorModel
{
    private Random _random;

    public MazeGeneratorModel(Random random)
    {
        _random = random;
    }
    
    
    public MazeElement[,] GenerateMaze(int rows, int cols)
    {
        MazeElement[,] maze = new MazeElement[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                maze[i, j] = MazeElement.Wall;
            }
        }

        Tuple<int, int> startCell = Tuple.Create(1, 1);
        Dfs(maze, startCell);

        MakeHoles(maze);
        RemoveLonelySquares(maze);

        return maze;
    }

    private void MakeHoles(MazeElement[,] maze)
    {

        for (int row = 1; row < maze.GetLength(0) - 1; row++)
        {
            for (int column = 1; column < maze.GetLength(1) - 1; column++)
            {
                if (column % 2 == 0 && column % 2 == 0) continue;

                bool isNeedRemove = _random.Next(100) < 20;

                if (!isNeedRemove) continue;

                maze[row, column] = MazeElement.Air;
            }
        }
    }

    private void RemoveLonelySquares(MazeElement[,] maze)
    {
        for (int row = 1; row < maze.GetLength(0) - 1; row++)
        {
            for (int column = 1; column < maze.GetLength(1) - 1; column++)
            {
                if (column % 2 != 0 || column % 2 != 0) continue;

                Vector2I currentPosition = new Vector2I(row, column);

                if (!IsWall(currentPosition, maze)) continue;

                Vector2I left = currentPosition + Vector2I.Left;
                Vector2I right = currentPosition + Vector2I.Right;
                Vector2I up = currentPosition + Vector2I.Up;
                Vector2I down = currentPosition + Vector2I.Down;

                if (IsWall(left, maze) || IsWall(right, maze) || IsWall(up, maze) || IsWall(down, maze)) continue;

                maze[row, column] = MazeElement.Air;
            }
        }
    }

    private bool IsWall(Vector2I position, MazeElement[,] maze)
    {
        if (!IsInsideMaze(position.X, position.Y, maze.GetLength(0), maze.GetLength(1))) return false;

        return maze[position.X, position.Y] == MazeElement.Wall;
    }

    private void Dfs(MazeElement[,] maze, Tuple<int, int> currentCell)
    {
        int row = currentCell.Item1;
        int col = currentCell.Item2;
        maze[row, col] = 0;

        int[] directions = { 0, 1, 2, 3 };
        Shuffle(directions);

        foreach (int direction in directions)
        {
            int[] dr = { 0, 1, 0, -1 };
            int[] dc = { 1, 0, -1, 0 };
            int newRow = row + 2 * dr[direction];
            int newCol = col + 2 * dc[direction];

            if (IsInsideMaze(newRow, newCol, maze.GetLength(0), maze.GetLength(1)) &&
                maze[newRow, newCol] == MazeElement.Wall)
            {
                maze[row + dr[direction], col + dc[direction]] = MazeElement.Air;
                Dfs(maze, Tuple.Create(newRow, newCol));
            }
        }
    }

    private void Shuffle(IList<int> array)
    {
        int n = array.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    private bool IsInsideMaze(int row, int col, int numRows, int numCols)
    {
        return row >= 0 && row < numRows && col >= 0 && col < numCols;
    }
}