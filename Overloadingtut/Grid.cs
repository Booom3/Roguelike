using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overloadingtut
{
    class Grid
    {
        private int height;
        private int width;
        private Tile[,] ground;
        private List<GameObject> gameObjects;
        private Sign[,] display;
        private char outofboundschar = 'X';
        private Position lastObjectCheckPos = new Position(0,0);
        private int maxActiveDistance = 200;
        private int recheckActiveObjectsDistance = 30;

        //Active > Visible
        private List<GameObject> activeObjects = new List<GameObject>();
        private List<GameObject> visibleObjects = new List<GameObject>();

        //Clear for GC purposes
        public void NullForGC()
        {
            ground = null;
            gameObjects = null;
            visibleObjects = null;
            display = null;
        }

        public int NumberOfGameObjects()
        {
            return gameObjects.Count;
        }

        public bool PosIsAvailable(Position Pos)
        {
            if (IsPointOnGrid(Pos))
            {
                if (ground[Pos.x, Pos.y].enterable)
                {
                    foreach(GameObject go in gameObjects)
                    {
                        if (go.position == Pos)
                            return false;
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public void RebuildDisplayGrid(Position Pos, int Width, int Height)
        {
            RebuildVisibleObjects(Pos, Width, Height);
            display = new Sign[Width * 2, Height * 2];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    display[x,y] = GetCharAtPoint(new Position(     (Pos.x - (int) Math.Floor(Width / 2.0)) + x,
                                                                    (Pos.y - (int) Math.Floor(Height / 2.0)) + y));
                }
            }
        }

        private void RebuildActiveObjects(Position Pos)
        {
            foreach (GameObject go in activeObjects)
                go.active = false;
            if (activeObjects.Count == 0 || Position.Distance(Pos, lastObjectCheckPos) > recheckActiveObjectsDistance)
            {
                lastObjectCheckPos = Pos;
                activeObjects.Clear();
                foreach (GameObject go in gameObjects)
                {
                    if (Position.Distance(Pos, go.position) < maxActiveDistance)
                        activeObjects.Add(go);
                }
            }
        }

        private void RebuildVisibleObjects(Position Pos, int Width, int Height)
        {
            RebuildActiveObjects(Pos);
            visibleObjects.Clear();
            foreach (GameObject go in activeObjects)
            {
                if (go.position.x < Pos.x + Width && go.position.x > Pos.x - Width - 1 &&
                    go.position.y < Pos.y + Height && go.position.y > Pos.y - Height - 1)
                    visibleObjects.Add(go);
            }
        }

        private Sign GetCharAtPoint(Position Pos)
        {
            Sign ret;
            if (IsPointOnGrid(Pos))
            {
                IEnumerable<GameObject> objectlist = 
                    from o in visibleObjects
                    where o.position == Pos
                    select o;
                if (objectlist.Any())
                {
                    ret.sign = objectlist.First().sign.sign;
                    ret.color = objectlist.First().sign.color;
                }
                else
                {
                    ret.sign = ground[Pos.x, Pos.y].sign.sign;
                    ret.color = ground[Pos.x, Pos.y].sign.color;
                }
            }
            else
            {
                ret.sign = outofboundschar;
                ret.color = ConsoleColor.DarkGreen;
            }
            return ret;
        }

        public bool IsPointOnGrid(Position Pos)
        {
            if (Pos.x < width && Pos.x >= 0 && Pos.y < height && Pos.y >= 0)
                return true;
            else
                return false;
        }

        public void DisplayPart(Position Pos, int Width, int Height)
        {
            RebuildDisplayGrid(Pos, Width, Height);
            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.ForegroundColor = display[x, y].color;
                    Console.Write(display[x, y].sign);
                }
                Console.Write("\n");
            }
        }

        Char[] floortiles = { '.', '-', ',', '\'' };
        ConsoleColor[] floorcolors = { ConsoleColor.DarkGreen, ConsoleColor.Green };
        private void MakeGrid()
        {
            ground = new Tile[width, height];
            gameObjects = new List<GameObject>();
            Random tileRandom = new Random();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    ground[x, y] = new Tile(floortiles[tileRandom.Next(floortiles.Length)], true, floorcolors[tileRandom.Next(floorcolors.Length)]);
        }
        public Grid(int Width, int Height)
        {
            height = Height;
            width = Width;
            MakeGrid();
        }

        public void AddGameObject(GameObject AddMe)
        {
            gameObjects.Add(AddMe);
        }

    }
}
