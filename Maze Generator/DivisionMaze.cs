using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maze_Generator
{
    class DivisionMaze
    {
        private const int HORIZONTAL = 0;
        private const int VERTICAL = 1;

        private const int E = 1 << 1;
        private const int S = 1 << 2;

        private static Random rand;

        private enum Orientation
        {
            HORIZONTAL, VERTICAL
        }

        public static void CreateMaze(int width, int height)
        {
            var seed = Guid.NewGuid().GetHashCode();
            rand = new Random(seed);

            var maze = CreateEmptyMaze(width, height);
            DivideMaze(maze, 0, 0, width, height, ChooseDivision(width, height));

            PrintMazeCommandLine(maze);
            PrintMazePNG(maze);
            Console.WriteLine($"{width} x {height} - Seed: {seed}");
        }

        private static int[,] CreateEmptyMaze(int width, int height)
        {
            var maze = new int[width, height];
   
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++) {
                    if (x != width - 1) maze[x, y] |= E;
                    if (y != height - 1) maze[x, y] |= S;
                }
            }

            return maze;
        }

        private static void DivideMaze(int[,] maze, int x, int y, int width, int height, int orientation)
        {
            if (width < 2 || height < 2)
            {
                return;
            }

            PrintMazeCommandLine(maze);
            Thread.Sleep(100);

            var isHorizontal = (orientation == HORIZONTAL);

            // Choose random wall and hole
            var wallX = x + (isHorizontal ? 0 : rand.Next(width - 1));
            var wallY = y + (isHorizontal ? rand.Next(height - 1) : 0);

            var holeX = wallX + (isHorizontal ? rand.Next(width) : 0);
            var holeY = wallY + (isHorizontal ? 0 : rand.Next(height));

            var dx = isHorizontal ? 1 : 0;
            var dy = isHorizontal ? 0 : 1;

            // Draw wall
            var length = isHorizontal ? width : height;
            var direction = isHorizontal ? S : E;

            for (int i = 0; i < length; i++)
            {
                if (wallX != holeX || wallY != holeY)
                {
                    maze[wallX, wallY] &= ~direction;
                }

                wallX += dx;
                wallY += dy;
            }

            // Recurse on either side of new wall
            var newWidth = isHorizontal ? width : wallX - x + 1;
            var newHeight = isHorizontal ? wallY - y + 1 : height;
            DivideMaze(maze, x, y, newWidth, newHeight, ChooseDivision(newWidth, newHeight));

            var newX = isHorizontal ? x : wallX + 1;
            var newY = isHorizontal ? wallY + 1: y;
            newWidth = isHorizontal ? width : x + width - wallX - 1;
            newHeight = isHorizontal ? y + height - wallY - 1 : height;
            DivideMaze(maze, newX, newY, newWidth, newHeight, ChooseDivision(newHeight, newWidth));
        }

        private static int ChooseDivision(int width, int height)
        {
            if (width < height)
            {
                return HORIZONTAL;
            }
            else if (width > height)
            {
                return VERTICAL;
            }
            else
            {
                return rand.Next(2) == 0 ? VERTICAL : HORIZONTAL;
            }
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

            mazeImage.Save("DivisionMaze.png", ImageFormat.Png);
        }
    }
}
