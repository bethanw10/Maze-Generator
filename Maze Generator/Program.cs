using System;

namespace Maze_Generator
{
    public class MazeGenerator {
        private static void Main(string[] args) {
            const int height = 10;
            const int width = 10;

            Console.WindowHeight = Math.Min(height + 4, 64);
            Console.WindowWidth = Math.Min(width * 2 + 2, 64);

            do {
                HuntAndKillMaze.CreateMaze(width, height);
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
