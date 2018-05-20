using System;
using System.Threading;

namespace Maze_Generator
{
    class DivisionMaze
    {
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

            Common.PrintMazeCommandLine(maze);
            Common.PrintMazePNG(maze, "DivisionMaze");
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

        private static void DivideMaze(int[,] maze, int x, int y, int width, int height, Orientation orientation)
        {
            if (width < 2 || height < 2)
            {
                return;
            }

            Common.PrintMazeCommandLine(maze);
            Thread.Sleep(100);

            var isHorizontal = (orientation == Orientation.HORIZONTAL);

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

        private static Orientation ChooseDivision(int width, int height)
        {
            if (width < height)
            {
                return Orientation.HORIZONTAL;
            }
            else if (width > height)
            {
                return Orientation.VERTICAL;
            }
            else
            {
                return rand.Next(2) == 0 ? Orientation.VERTICAL : Orientation.HORIZONTAL;
            }
        }
        
    }
}
