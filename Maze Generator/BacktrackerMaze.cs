using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Maze_Generator
{
    public static class BackTrackerMaze {

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

        private static Random rand;

        public static void CreateMaze(int size) {
            CreateMaze(size, size);
        }

        public static void CreateMaze(int width, int height) {
            int seed = DateTime.Now.Millisecond;
            rand = new Random(seed);

            var maze = new int[width, height];
            CreatePathFrom(0, 0, maze);

            Common.PrintMazeCommandLine(maze);
            Console.WriteLine($"{width} x {height} - Seed: {seed}");

            Common.PrintMazePNG(maze, "BacktrackerMaze");
        }

        private static void CreatePathFrom(int x, int y, int[,] maze) {
            var directions = new[] {N, E, S, W}.OrderBy(n => rand.Next());

            foreach (var direction in directions) {
                var nextX = x + DX[direction];
                var nextY = y + DY[direction];

                if (Common.IsInBounds(nextX, nextY, maze) && maze[nextX, nextY] == 0) {
                    maze[x, y] |= direction;
                    maze[nextX, nextY] |= opposite[direction];

                    Thread.Sleep(25);
                    Common.PrintMazeCommandLine(maze, nextX, nextY);
                    CreatePathFrom(nextX, nextY, maze);
                }
            }
        }
    }
}