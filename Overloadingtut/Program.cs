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
        private static int _displayWidth = 19;
        private static int _displayHeight = 19;

        private static int _nextScreen  = 1;
        private const int QUIT          = -1;
        private const int MAINGAME      = 0;
        private const int MAINMENU      = 1;

        static int MainMenuGetInt(string Text)
        {
            while (true)
            {
                Console.WriteLine(Text);
                string tempin = Console.ReadLine();
                int ret;
                try
                {
                    ret = Convert.ToInt32(tempin);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Invalid number.");
                    continue;
                }
                return ret;
            }
        }

        static void MainMenu()
        {
            Console.Clear();
            bool Running = true;
            string A = "A";
            string B = "B";
            string C = "C";
            string Done = "DONE";
            while (Running)
            {
                Console.Write(  "(" + A + ") Change number of game objects\n" +
                                "(" + B + ") Change grid size\n" + 
                                "(" + C + ") Change viewport size\n" +
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
                    _gridWidth = MainMenuGetInt("Enter new width:");
                    _gridHeight = MainMenuGetInt("Enter new height:");
                }
                else if (entry == C)
                {
                    while ((_displayWidth = MainMenuGetInt("Enter new viewport width (Odd number):")) % 2 == 0)
                        Console.WriteLine("Not odd.");
                    while ((_displayHeight = MainMenuGetInt("Enter new viewport height (Odd number):")) % 2 == 0)
                        Console.WriteLine("Not odd.");
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

                //Draw debug data
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.SetCursorPosition(0, _displayHeight+1);
                Console.Write(DateTime.Now + "\n");
                Console.Write("Next update " + nextupdate + ":" + nextupdate.Millisecond + "\n");
                Console.Write("Waiting " + (nextupdate - DateTime.Now) + "ms.\n");

                //Set the cursor back 3 lines, if it waits to draw the next frame
                //this will make it overwrite the debug lines.
                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 3);


                if (DateTime.Now < nextupdate)
                {
                    Thread.Sleep(0);


                    continue;
                }



                string inputraw = "";
                //Only read input if there is input.
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
                    //Clear all buffered keys.
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                }

                Console.SetCursorPosition(0, 0);
                grid.DisplayPart(g.position, _displayWidth, _displayHeight);


                framerate += 1;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.SetCursorPosition(0, _displayHeight);


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
            //Clear out references so GC can do its thing in Main()
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
