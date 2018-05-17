using System;

namespace Maze_Generator
{
    public class MazeGenerator
    {
        private static void Main(string[] args)
        {
            const int height = 5;
            const int width = 20;

            Console.WindowHeight = height + 4;
            Console.WindowWidth = width * 2 + 2;

            do
            {
                BackTrackerMaze.CreateMaze(width, height);
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }

}
