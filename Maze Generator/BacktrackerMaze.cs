using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

            PrintMazeCommandLine(maze);
            Console.WriteLine($"{width} x {height} - Seed: {seed}");

            PrintMazePNG(maze);
        }

        private static void CreatePathFrom(int x, int y, int[,] maze) {
            var directions = new[] {N, E, S, W}.OrderBy(n => rand.Next());

            foreach (var direction in directions) {
                var nextX = x + DX[direction];
                var nextY = y + DY[direction];

                if (IsInBounds(nextX, nextY, maze) && maze[nextX, nextY] == 0) {
                    maze[x, y] |= direction;
                    maze[nextX, nextY] |= opposite[direction];

                    Thread.Sleep(25);
                    PrintMazeCommandLine(maze, nextX, nextY);
                    CreatePathFrom(nextX, nextY, maze);
                }
            }
        }

        private static bool IsInBounds(int x, int y, int[,] grid) {
            return x >= 0 && x <= grid.GetLength(0) - 1 && 
                   y >= 0 && y <= grid.GetLength(1) - 1;
        }

        private static void PrintMazeCommandLine(int[,] maze) {
            Console.Clear();

            Console.WriteLine(" " + new string('_', maze.GetLength(0) * 2 - 1));
            for (var y = 0; y < maze.GetLength(1); y++)
            {
                Console.Write("|");
                for (var x = 0; x < maze.GetLength(0); x++)
                {
                    // Bottom wall
                    Console.Write((maze[x, y] & S) != 0 ? " " : "_");

                    if ((maze[x, y] & E) != 0)
                    {
                        // Connecting bottom wall
                        Console.Write(((maze[x, y] | maze[x + 1, y]) & S) == 0 ? "_" : " ");
                    }
                    else
                    {
                        // Right wall
                        Console.Write("|");
                    }
                }

                Console.WriteLine();
            }
        }

        private static void PrintMazeCommandLine(int[,] maze, int currentX, int currentY) {
            Console.Clear();

            Console.WriteLine(" " + new string('_', maze.GetLength(0) * 2 - 1));
            for (var y = 0; y < maze.GetLength(1); y++) {
                Console.Write("|");
                for (var x = 0; x < maze.GetLength(0); x++) {
                    // Bottom wall
                    if (x == currentX && y == currentY) {
                        Console.Write("X");
                    } else {
                        Console.Write((maze[x, y] & S) != 0 ? " " : "_");
                    }

                    if ((maze[x, y] & E) != 0) {
                        // Connecting bottom wall
                        Console.Write(((maze[x, y] | maze[x + 1, y]) & S) == 0 ? "_" : " ");
                    } else {
                        // Right wall
                        Console.Write("|");
                    }
                }

                Console.WriteLine();
            }
        }

        private static void PrintMazePNG(int[,] maze)
        {
            Bitmap mazeImage = new Bitmap(maze.GetLength(0) * 2 + 1, maze.GetLength(1) * 2 + 1, PixelFormat.Format24bppRgb);

            // Top border
            for (int i = 0; i < mazeImage.Width; i++) {
                mazeImage.SetPixel(i, 0, Color.Black);
            }
            
            for (var x = 0; x < maze.GetLength(0); x++) {
                var cellX = 2 * x + 1;

                // Left border
                mazeImage.SetPixel(cellX, 0, Color.Black);

                for (var y = 0; y < maze.GetLength(1); y++) {
                    var cellY = 2 * y + 1;

                    // Cell
                    mazeImage.SetPixel(cellX, cellY, Color.White);

                    // Right wall
                    mazeImage.SetPixel(cellX + 1, cellY, (maze[x, y] & E) == 0 ? Color.Black : Color.White);

                    // Bottom wall
                    mazeImage.SetPixel(cellX, cellY + 1, (maze[x, y] & S) == 0 ? Color.Black : Color.White);

                    // Bottom Right wall
                    mazeImage.SetPixel(cellX + 1, cellY + 1, Color.Black);
                }      
                
            }

            mazeImage.Save("BrackTackerMaze.png", ImageFormat.Png);
        }
    }
}