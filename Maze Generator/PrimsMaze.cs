using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Maze_Generator
{
    class PrimsMaze
    {
        private const int N = 1 << 0;
        private const int E = 1 << 1;
        private const int S = 1 << 2;
        private const int W = 1 << 3;

        private const int IN = 1 << 4;
        private const int FRONTIER = 1 << 5;

        private static readonly Dictionary<int, int> opposite = new Dictionary<int, int> {
            {N, S}, {E, W}, {S, N}, {W, E}
        };

        public static void CreateMaze(int width, int height)
        {
            var seed = Guid.NewGuid().GetHashCode();
            var rand = new Random(seed);

            var maze = new int[width, height];
            var frontier = new List<int[]>();

            // Pick random cell to be starting point
            MarkAsIn(rand.Next(width -1), rand.Next(height - 1), maze, frontier);
            var directions = new[] { N, E, S, W };

            while (frontier.Count != 0)
            {
                // Get random cell from frontier
                var cell = GetRandomFrontier(frontier, rand);
                var x = cell[0];
                var y = cell[1];

                // Choose random neighbour
                var n = GetInNeighbours(x, y, maze);
                var randomN = (int[])n[rand.Next(n.Count)];
                var nx = randomN[0];
                var ny = randomN[1];

                // Create path
                var direction = GetDirection(x, y, nx, ny);
                maze[x, y] |= direction;
                maze[nx, ny] |= opposite[direction];

                // Mark cell as IN
                MarkAsIn(x, y, maze, frontier);

                Thread.Sleep(10);
                Common.PrintMazeCommandLine(maze);
            }

            Common.PrintMazePNG(maze, "PrimsMaze");
            Console.WriteLine($"{width} x {height} - Seed: {seed}");
        }

        private static void AddFrontier(int x, int y, int[,] grid, List<int[]> frontier)
        {
            if (Common.IsInBounds(x, y, grid))
            {
                if ((grid[x, y] & FRONTIER) != 0)
                {
                    return;
                }

                grid[x, y] |= FRONTIER;
                frontier.Add(new int[] { x, y });
            }
        }

        // Mark a cell as “in” (which then marks the “out” neighbors as frontier cells)
        private static void MarkAsIn(int x, int y, int[,] grid, List<int[]> frontier)
        {
            grid[x, y] |= IN;
            AddFrontier(x - 1, y, grid, frontier);
            AddFrontier(x + 1, y, grid, frontier);
            AddFrontier(x, y - 1, grid, frontier);
            AddFrontier(x, y + 1, grid, frontier);
        }
        
        // Returns all the “in” neighbors of a given frontier cell
        private static ArrayList GetInNeighbours(int x, int y, int[,] grid)
        {
            ArrayList neighbours = new ArrayList();
            
            if (IsIn(x - 1, y, grid)) neighbours.Add(new int[] { x - 1, y });
            if (IsIn(x + 1, y, grid)) neighbours.Add(new int[] { x + 1, y });
            if (IsIn(x, y - 1, grid)) neighbours.Add(new int[] { x, y - 1 });
            if (IsIn(x, y + 1, grid)) neighbours.Add(new int[] { x, y + 1 });

            return neighbours;
        }

        private static bool IsIn(int x, int y, int[,] grid)
        {
            return Common.IsInBounds(x, y, grid) && (grid[x, y] & IN) != 0;
        }

        private static int GetDirection(int srcX, int srcY, int destX, int destY)
        {
            if (srcX < destX) return E;
            if (srcX > destX) return W;
            if (srcY < destY) return S;
            if (srcY > destY) return N;

            return 0;
        }

        private static int[] GetRandomFrontier(List<int[]> frontier, Random rand)
        {
            var i = rand.Next(frontier.Count);
            var cell = frontier[i];
            frontier.RemoveAt(i);
            return cell;
        }

    }
}
