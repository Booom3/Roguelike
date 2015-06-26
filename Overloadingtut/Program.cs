using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Overloadingtut
{
    class Program
    {

        public static Grunt g;
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.CursorVisible = false;
            string left = "A";
            string right = "D";
            string up = "W";
            string down = "S";
            Grid grid = new Grid(1000, 1000);
            g = GameObject.Instantiate<Grunt>(new Position(4, 7), grid);
            g.sign.sign = 'M';
            g.sign.color = ConsoleColor.Red;
            Random rand = new Random();
            ConsoleColor[] enemycolors = { ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.Yellow };
            for (int i = 0; i < 3000000; i++)
            {
                GameObject gtemp = GameObject.Instantiate<Grunt>(new Position(rand.Next(3980), 8+rand.Next(3980)), grid);
                gtemp.sign.color = enemycolors[rand.Next(enemycolors.Length)];
            }
            bool Running = true;
            DateTime nextupdate = DateTime.Now;
            int minsleeptime = 16;
            int framerate = 0;
            int lastFramerate = 0;
            DateTime framerateCounter = DateTime.Now;
            while (Running)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.SetCursorPosition(0, 21);
                Console.Write(DateTime.Now + "\n");
                Console.Write("Next update " + nextupdate + ":" + nextupdate.Millisecond + "\n");
                Console.Write("Waiting " + (nextupdate - DateTime.Now) + "ms.\n");
                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 3);
                if (DateTime.Now < nextupdate)
                {
                    Thread.Sleep(0);
                    

                    continue;
                }
                
                Console.SetCursorPosition(0, 0);
                grid.DisplayPart(g.position, 10, 10);

                string inputraw = "";
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    inputraw = key.KeyChar.ToString();
                    inputraw = inputraw.ToUpper();
                    if (inputraw == up)
                    {
                        g.Move(0, 1);
                    }
                    else if (inputraw == down)
                    {
                        g.Move(0, -1);
                    }
                    else if (inputraw == right)
                    {
                        g.Move(1, 0);
                    }
                    else if (inputraw == left)
                    {
                        g.Move(-1, 0);
                    }
                    else if (inputraw == "X")
                    {
                        Running = false;
                    }
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                }
                framerate += 1;
                Console.ForegroundColor = ConsoleColor.DarkRed;

                if (DateTime.Now > framerateCounter)
                {
                    Console.Write(framerate.ToString("D3"));
                    lastFramerate = framerate;
                    framerate = 0;
                    framerateCounter = framerateCounter.AddSeconds(1);
                }
                else
                    Console.Write(lastFramerate.ToString("D3"));

                Console.Write("FPS");
                Console.Write(" Current game objects: " + grid.NumberOfGameObjects() + "\n");
                if (DateTime.Compare(DateTime.Now, nextupdate.AddMilliseconds(minsleeptime)) < 0)
                    nextupdate = nextupdate.AddMilliseconds(minsleeptime);
                else
                    nextupdate = DateTime.Now;
            }
        }
    }
}
