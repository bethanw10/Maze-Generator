using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Maze_Generator
{
    class HuntAndKillMaze
    {
        // Path directions
        private const int N = 1 << 0;
        private const int E = 1 << 1;
        private const int S = 1 << 2;
        private const int W = 1 << 3;

        private static readonly Dictionary<int, int> DX = new Dictionary<int, int> {
            {N, 0}, {E, 1}, {S, 0}, {W, -1}
        };

        private static readonly Dictionary<int, int> DY = new Dictionary<int, int> {
            {N, -1}, {E, 0}, {S, 1}, {W, 0}
        };

        private static readonly Dictionary<int, int> opposite = new Dictionary<int, int> {
            {N, S}, {E, W}, {S, N}, {W, E}
        };

        private const int THREAD_SLEEP = 50;

        private static int huntStart;

        public static void CreateMaze(int width, int height)
        {
            Console.Title = "Hunt And Kill Maze";

            int seed = DateTime.Now.Millisecond;
            var rand = new Random(seed);

            var maze = new int[width, height];
            var x = rand.Next(width);
            var y = rand.Next(height);
            huntStart = x;

            do
            {
                Common.PrintMazeCommandLine(maze, x, y);
                Thread.Sleep(THREAD_SLEEP);

                var nextCell = Walk(x, y, maze, rand);
                
                if (nextCell is null) // Walk has ended, 'hunt' for next starting cell 
                {
                    nextCell = Hunt(maze, rand);
                    if (nextCell is null) break;
                }
                
                x = nextCell[0];
                y = nextCell[1];
            } while (true);

            Common.PrintMazeCommandLine(maze);
            Console.WriteLine($"{width} x {height} - Seed: {seed}");

            Common.PrintMazePNG(maze, "HuntAndKillMaze", 5, 2);
        }

        // Create path in a random direction. Returns null if there was no viable path to take
        private static int[] Walk(int x, int y, int[,] maze, Random rand)
        {
            var directions = new[] { N, E, S, W }.OrderBy(n => rand.Next());

            foreach (var direction in directions)
            {
                var nextX = x + DX[direction];
                var nextY = y + DY[direction];

                if (Common.IsInBounds(nextX, nextY, maze) && maze[nextX, nextY] == 0)
                {
                    maze[x, y] |= direction;
                    maze[nextX, nextY] |= opposite[direction];

                    // Record highest point reached as the new hunt start point
                    if (nextY < huntStart) huntStart = nextY;

                    return new int[] { nextX, nextY };
                }
            }

            return null;
        }

        // Looks for first empty cell with at least one visited neighbour
        private static int[] Hunt(int[,] maze, Random rand)
        {
            for (int y = huntStart; y < maze.GetLength(1); y++)
            {
                var visitedCellCount = 0;

                for (int x = 0; x < maze.GetLength(0); x++)
                {
                    Common.PrintMazeCommandLine(maze, x, y, '?');
                    Thread.Sleep(THREAD_SLEEP / 2);

                    // Ignore visited cells
                    if (maze[x, y] != 0)
                    {
                        visitedCellCount++;

                        // If entire line has been visited
                        if (x == maze.GetLength(0) - 1 && visitedCellCount == maze.GetLength(0))
                        {
                            // Record as new row start to avoid rechecking complete rows
                            huntStart = y + 1;
                        }

                        continue;
                    }

                    // Get list of all visited neighbours
                    var neighbours = new List<int>();
                    if (y > 0 && (maze[x, y - 1] != 0)) neighbours.Add(N);
                    if (x > 0 && (maze[x - 1, y] != 0)) neighbours.Add(W);
                    if (x + 1 < maze.GetLength(0) && (maze[x + 1, y] != 0)) neighbours.Add(E);
                    if (y + 1 < maze.GetLength(1) && (maze[x, y + 1] != 0)) neighbours.Add(S);

                    if (neighbours.Count == 0) continue;

                    var direction = neighbours[rand.Next(neighbours.Count)];

                    var newX = x + DX[direction];
                    var newY = y + DY[direction];

                    maze[x, y] |= direction;
                    maze[newX, newY] |= opposite[direction];

                    return new int[] { x, y };
                }
            }

            return null;
        }
        
    }

}
