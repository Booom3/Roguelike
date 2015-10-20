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
        private int fullRecheckActiveObjectsdistance = 60;

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
                if (go.Position == Pos)
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
                        if (go.Position == Pos && go.enterable == false)
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
            foreach (GameObject g in activeObjects)
                g.Active = false;
            activeObjects.Clear();
        }

        private Position lastCheckPosition = new Position(0, 0);

        //This is used when you move stuff other than the player object around.
        public bool IsInRangeOfLastActiveCheck(GameObject gameObject)
        {
            if (activeObjects.Count == 0)
                return false;
            if (IsInActiveRange(gameObject.Position, lastCheckPosition))
            {
                if (!activeObjects.Contains(gameObject))
                {
                    activeObjects.Add(gameObject);
                }
                return true;
            }
            else
            {
                if (activeObjects.Contains(gameObject))
                {
                    activeObjects.Remove(gameObject);
                }
                return false;
            }
        }

        //Keeps track of where in the list of gameobjects we currently are while rebuilding
        int CurrentBatchRebuild = 0;

        //How many gameobjects to process each pass
        int BatchSize = 400000;

        List<GameObject> BatchObjects = new List<GameObject>();
        bool BatchComplete = false;
        private void RebuildObjects(Position Pos, int Width, int Height)
        {
            //Ceiling all the ranges for the sake of consistency
            Width = (int)Math.Ceiling(Width / 2.0);
            Height = (int)Math.Ceiling(Height / 2.0);
            lastCheckPosition = Pos;
            visibleObjects.Clear();

            //This rebuilds the entire active objects list in one pass.
            //If there is only 1 active object and this is being called (this happens when you move the player to a grid, as all active objects
            //get deactivated when you move off a grid) then rebuild the entire thing immediately.
            //Alternatively if the player has moved too far from their previous position AND the batch updating hasn't finished yet, do it all
            //in one go instead. Lags a bit more but avoids a pop-in effect of gameobjects being reactivated on screen.
            if (activeObjects.Count <= 1 || Position.Distance(Pos, lastObjectCheckPos) > fullRecheckActiveObjectsdistance)
            {
                foreach (GameObject go in activeObjects)
                    go.Active = false;
                activeObjects.Clear();
                lastObjectCheckPos = Pos;
                BatchObjects.Clear();
                CurrentBatchRebuild = 0;
                BatchComplete = false;
                foreach (GameObject go in gameObjects)
                {
                    if (IsInActiveRange(go.Position, Pos))
                    {
                        activeObjects.Add(go);
                        go.Active = true;
                        if (IsInVisibleRange(go.Position, Pos, Width, Height))
                        {
                            visibleObjects.Add(go);
                        }
                    }
                }
            }
            else if (Position.Distance(Pos, lastObjectCheckPos) > recheckActiveObjectsDistance)
            {
                for (int i = 0; i <= BatchSize; i++)
                {
                    if (i+CurrentBatchRebuild >= gameObjects.Count)
                    {
                        BatchComplete = true;
                        break;
                    }
                    if (gameObjects[i+CurrentBatchRebuild] != null)
                    {
                        if (IsInActiveRange(gameObjects[i+CurrentBatchRebuild].Position, Pos))
                        {
                            BatchObjects.Add(gameObjects[i+CurrentBatchRebuild]);
                        }
                    }
                    if (i >= BatchSize)
                    {
                        CurrentBatchRebuild = i + CurrentBatchRebuild;
                    }
                }
                if (BatchComplete)
                {
                    activeObjects.Clear();
                    foreach (GameObject go in BatchObjects)
                        activeObjects.Add(go);
                    lastObjectCheckPos = Pos;
                    BatchObjects.Clear();
                    CurrentBatchRebuild = 0;
                    BatchComplete = false;
                }
                foreach (GameObject go in activeObjects)
                    if (IsInVisibleRange(go.Position, Pos, Width, Height))
                        visibleObjects.Add(go);
            }
            else
            {
                foreach (GameObject go in activeObjects)
                {
                    if (IsInVisibleRange(go.Position, Pos, Width, Height))
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

            if (ObjectPos.x < Pos.x + Width && ObjectPos.x > Pos.x - Width &&
                ObjectPos.y < Pos.y + Height && ObjectPos.y > Pos.y - Height)
                return true;
            else
                return false;
        }

        private Sign GetCharAtVisiblePoint(Position Pos)
        {
            Sign ret;
            IEnumerable<GameObject> objectlist = 
                from o in visibleObjects
                where o.Position == Pos
                select o;
            if (objectlist.Any())
            {
                IEnumerable<GameObject> sortvisible =
                    from o in objectlist
                    where o.enterable == false
                    select o;
                if (sortvisible.Any())
                {
                    ret = sortvisible.First().sign;
                }
                else
                {
                    ret = objectlist.First().sign;
                }
            }
            else if (IsPointOnGrid(Pos))
            {
                ret = ground[Pos.x, Pos.y].sign;
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
