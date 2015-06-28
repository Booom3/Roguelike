using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overloadingtut
{
    class Grid
    {
        public Position worldPosition = new Position(0, 0);
        private int height;
        private int width;
        private Tile[,] ground;
        private List<GameObject> gameObjects;
        private Sign[,] display;
        private char outofboundschar = 'X';
        private Position lastObjectCheckPos = new Position(0,0);
        private int maxActiveDistance = 100;
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

        public IUsable GetUsableOnPos(Position Pos)
        {
            foreach(GameObject go in activeObjects)
            {
                if (go.position == Pos)
                {
                    IUsable ius = go as IUsable;
                    if (ius != null)
                    {
                        return ius;
                    }
                }
            }
            return null;
        }

        public bool PosIsAvailable(Position Pos, bool AllObjects)
        {
            if (IsPointOnGrid(Pos))
            {
                if (ground[Pos.x, Pos.y].enterable)
                {
                    foreach(GameObject go in (AllObjects ? gameObjects : activeObjects))
                    {
                        if (go.position == Pos && go.enterable == false)
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
            RebuildObjects(Pos, Width, Height);
            display = new Sign[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    display[x,y] = GetCharAtVisiblePoint(new Position(     (Pos.x - (int) Math.Floor(Width / 2.0)) + x,
                                                                    (Pos.y - (int) Math.Floor(Height / 2.0)) + y));
                }
            }
        }

        public void ClearActiveObjects()
        {
            activeObjects.Clear();
        }

        private void RebuildObjects(Position Pos, int Width, int Height)
        {
            Width = (int)Math.Floor(Width / 2.0);
            Height = (int)Math.Floor(Height / 2.0);
            visibleObjects.Clear();
            if (activeObjects.Count <= 1 || Position.Distance(Pos, lastObjectCheckPos) > recheckActiveObjectsDistance)
            {
                foreach (GameObject go in activeObjects)
                    go.active = false;
                activeObjects.Clear();
                lastObjectCheckPos = Pos;
                foreach (GameObject go in gameObjects)
                {
                    if (IsInActiveRange(go.position, Pos))
                    {
                        activeObjects.Add(go);
                        go.active = true;
                        if (IsInVisibleRange(go.position, Pos, Width, Height))
                        {
                            visibleObjects.Add(go);
                        }
                    }
                }
            }
            else
            {
                foreach (GameObject go in activeObjects)
                {
                    if (IsInVisibleRange(go.position, Pos, Width, Height))
                    {
                        visibleObjects.Add(go);
                    }
                }
            }
        }

        private bool IsInActiveRange(Position ObjectPos, Position Pos)
        {
            if (Position.Distance(ObjectPos, Pos) < maxActiveDistance)
                return true;
            else
                return false;
        }

        private bool IsInVisibleRange(Position ObjectPos, Position Pos, int Width, int Height)
        {

            if (ObjectPos.x < Pos.x + Width  && ObjectPos.x > Pos.x - Width - 1 &&
                ObjectPos.y < Pos.y + Height && ObjectPos.y > Pos.y - Height - 1)
                return true;
            else
                return false;
        }

        private Sign GetCharAtVisiblePoint(Position Pos)
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
                    IEnumerable<GameObject> sortvisible =
                        from o in objectlist
                        where o.enterable == false
                        select o;
                    if (sortvisible.Any())
                    {
                        ret.sign = sortvisible.First().sign.sign;
                        ret.color = sortvisible.First().sign.color;
                    }
                    else
                    {
                        ret.sign = objectlist.First().sign.sign;
                        ret.color = objectlist.First().sign.color;
                    }
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

        public void MoveExistingGameObjectHere(GameObject MoveMe)
        {
            gameObjects.Add(MoveMe);
            activeObjects.Add(MoveMe);
        }

        public void AddNewGameObject(GameObject AddMe)
        {
            gameObjects.Add(AddMe);
        }

        public void RemoveGameObject(GameObject RemoveMe)
        {
            gameObjects.Remove(RemoveMe);
            activeObjects.Remove(RemoveMe);
            visibleObjects.Remove(RemoveMe);
        }

    }
}
