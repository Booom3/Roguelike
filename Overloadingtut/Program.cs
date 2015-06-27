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

        private static int _gridWidth = 900;
        private static int _gridHeight = 900;
        private static int _numEnemies = 30000;

        private static int _nextScreen  = 0;
        private const int QUIT          = -1;
        private const int MAINGAME      = 0;
        private const int MAINMENU      = 1;


        static void MainMenu()
        {
            Console.Clear();
            bool Running = true;
            string A = "A";
            string B = "B";
            string Done = "DONE";
            while (Running)
            {
                Console.Write(  "(" + A + ") Change number of game objects\n" +
                                "(" + B + ") Change grid size\n" + 
                                "(" + Done + ") Finish\n");
                string entry = Console.ReadLine();
                entry = entry.ToUpper();
                if (entry == A)
                {
                    Console.WriteLine("Enter new number of game objects:");
                    string numobjects = Console.ReadLine();
                    int num;
                    try
                    {
                        num = Convert.ToInt32(numobjects);
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Not a number.");
                        continue;
                    }
                    _numEnemies = num;
                    continue;
                }
                else if (entry == B)
                {
                    while (true)
                    {
                        Console.WriteLine("Enter new width:");
                        string newwidth = Console.ReadLine();
                        int width;
                        try
                        {
                            width = Convert.ToInt32(newwidth);
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine("Not a number.");
                            continue;
                        }
                        Console.WriteLine("Enter new height:");
                        string newheight = Console.ReadLine();
                        int height;
                        try
                        {
                            height = Convert.ToInt32(newheight);
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine("Not a number.");
                            continue;
                        }
                        _gridWidth = width;
                        _gridHeight = height;
                        break;
                    }
                }
                else if (entry == Done)
                {
                    Running = false;
                    _nextScreen = MAINGAME;
                }
            }
        }

        static void MainGame()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.CursorVisible = false;
            string left = "A";
            string right = "D";
            string up = "W";
            string down = "S";
            Grid grid = null;
            try
            {
                grid = new Grid(_gridWidth, _gridHeight);
                g = GameObject.Instantiate<Grunt>(new Position(4, 7), grid);
                g.sign.sign = 'M';
                g.sign.color = ConsoleColor.Red;
                Random rand = new Random();
                ConsoleColor[] enemycolors = { ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.Yellow };
                for (int i = 0; i < _numEnemies; i++)
                {
                    GameObject gtemp = GameObject.Instantiate<Grunt>(new Position(rand.Next(_gridWidth), rand.Next(_gridHeight)), grid);
                    gtemp.sign.color = enemycolors[rand.Next(enemycolors.Length)];
                }
            }
            catch (OutOfMemoryException ex)
            {
                Console.Clear();
                Console.WriteLine("I ran out of memory. I am terribly sorry.");
                Console.ReadKey();
                if (grid != null) grid.NullForGC();
                _nextScreen = MAINMENU;
                return;
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
                        _nextScreen = QUIT;
                        Running = false;
                    }
                    else if (inputraw == "M")
                    {
                        _nextScreen = MAINMENU;
                        Running = false;
                    }
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                }

                Console.SetCursorPosition(0, 0);
                grid.DisplayPart(g.position, 30, 10);


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
            grid.NullForGC();
            grid = null;
        }

        static void Main(string[] args)
        {
            bool Running = true;
            while (Running)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (_nextScreen == MAINGAME)
                {
                    MainGame();
                }
                else if (_nextScreen == MAINMENU)
                {
                    MainMenu();
                }
                else if (_nextScreen == QUIT)
                {
                    Running = false;
                }
            }
        }
    }
}
