using System;

namespace Maze_Generator
{
    public class MazeGenerator {
        private static void Main(string[] args) {
            const int height = 30;
            const int width = 30;

            Console.WindowHeight = Math.Min(height + 4, 64);
            Console.WindowWidth = Math.Min(width * 2 + 2, 64);

            do {
                //BackTrackerMaze.CreateMaze(width, height);
                //Console.ReadKey(true);

                //PrimsMaze.CreateMaze(width, height);
                //Console.ReadKey(true);

                //DivisionMaze.CreateMaze(width, height);
                //Console.ReadKey(true);
             
                HuntAndKillMaze.CreateMaze(width, height);
                Console.ReadKey(true);

            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
