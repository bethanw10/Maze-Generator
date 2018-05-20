using System;

namespace Maze_Generator
{
    public class MazeGenerator {
        private static void Main(string[] args) {
            const int height = 20;
            const int width = 20;

            Console.WindowHeight = Math.Min(height + 4, 64);
            Console.WindowWidth = Math.Min(width * 2 + 2, 64);

            do {
                //PrimsMaze.CreateMaze(width, height);
                //Console.ReadKey(true);

                //DivisionMaze.CreateMaze(width, height);
                //Console.ReadKey(true);
                         
                //BackTrackerMaze.CreateMaze(width, height);
                //Console.ReadKey(true);

                HuntAndKillMaze.CreateMaze(width, height);
                Console.ReadKey(true);

            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
