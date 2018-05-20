using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        private const int THREAD_SLEEP = 25;

        public static void CreateMaze(int width, int height)
        {
            int seed = DateTime.Now.Millisecond;
            var rand = new Random(seed);

            var maze = new int[width, height];
            var x = rand.Next(width);
            var y = rand.Next(height);

            do
            {
                PrintMazeCommandLine(maze, x, y);
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

            PrintMazeCommandLine(maze);
            Console.WriteLine($"{width} x {height} - Seed: {seed}");

            PrintMazePNG(maze);
        }

        // Create path in a random direction. Returns null if there was no viable path to take
        private static int[] Walk(int x, int y, int[,] maze, Random rand)
        {
            var directions = new[] { N, E, S, W }.OrderBy(n => rand.Next());

            foreach (var direction in directions)
            {
                var nextX = x + DX[direction];
                var nextY = y + DY[direction];

                if (IsInBounds(nextX, nextY, maze) && maze[nextX, nextY] == 0)
                {
                    maze[x, y] |= direction;
                    maze[nextX, nextY] |= opposite[direction];

                    return new int[] { nextX, nextY };
                }
            }

            return null;
        }

        // Looks for first empty cell with at least one visited neighbour
        private static int[] Hunt(int[,] maze, Random rand)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                for (int x = 0; x < maze.GetLength(0); x++)
                {
                    PrintMazeCommandLine(maze, x, y, '?');
                    Thread.Sleep(THREAD_SLEEP / 2);

                    // Ignore unvisited cells
                    if (maze[x, y] != 0) continue;

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

        private static bool IsInBounds(int x, int y, int[,] grid)
        {
            return x >= 0 && x <= grid.GetLength(0) - 1 &&
                   y >= 0 && y <= grid.GetLength(1) - 1;
        }

        private static void PrintMazeCommandLine(int[,] maze)
        {
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

        private static void PrintMazeCommandLine(int[,] maze, int currentX, int currentY, char character = 'X')
        {
            Console.Clear();

            Console.WriteLine(" " + new string('_', maze.GetLength(0) * 2 - 1));
            for (var y = 0; y < maze.GetLength(1); y++)
            {
                Console.Write("|");
                for (var x = 0; x < maze.GetLength(0); x++)
                {
                    // Bottom wall
                    if (x == currentX && y == currentY)
                    {
                        Console.Write(character);
                    }
                    else
                    {
                        Console.Write((maze[x, y] & S) != 0 ? " " : "_");
                    }

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

        private static void PrintMazePNG(int[,] maze)
        {
            Bitmap mazeImage = new Bitmap(maze.GetLength(0) * 2 + 1, maze.GetLength(1) * 2 + 1, PixelFormat.Format24bppRgb);

            // Top border
            for (int i = 0; i < mazeImage.Width; i++)
            {
                mazeImage.SetPixel(i, 0, Color.Black);
            }

            for (var x = 0; x < maze.GetLength(0); x++)
            {
                var cellX = 2 * x + 1;

                // Left border
                mazeImage.SetPixel(cellX, 0, Color.Black);

                for (var y = 0; y < maze.GetLength(1); y++)
                {
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

            mazeImage.Save("HuntAndKillMaze.png", ImageFormat.Png);
        }
    }

}
