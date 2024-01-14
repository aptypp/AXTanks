using System;
using System.Collections.Generic;

namespace AXTanks.Scripts;

internal class MazeGeneratorGpt
{
    public static MazeElement[,] GenerateMaze(int rows, int cols)
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

        return maze;
    }

    private static void Dfs(MazeElement[,] maze, Tuple<int, int> currentCell)
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

            if (IsInsideMaze(newRow, newCol, maze.GetLength(0), maze.GetLength(1)) && maze[newRow, newCol] == MazeElement.Wall)
            {
                maze[row + dr[direction], col + dc[direction]] = MazeElement.Air;
                Dfs(maze, Tuple.Create(newRow, newCol));
            }
        }
    }

    private static void Shuffle(IList<int> array)
    {
        Random random = new Random();
        int n = array.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    private static bool IsInsideMaze(int row, int col, int numRows, int numCols)
    {
        return row >= 0 && row < numRows && col >= 0 && col < numCols;
    }
}