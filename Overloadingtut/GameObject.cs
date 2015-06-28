using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overloadingtut
{
    class GameObject
    {
        public Position position;
        public Sign sign;
        public Grid attachedGrid;
        public bool active = false;
        public bool enterable = false;
        public bool player = false;

        public static T Instantiate<T>(Position position, Grid AttachToGrid) where T : GameObject
        {
            T temp = (T)Activator.CreateInstance(typeof(T));
            temp.position = new Position(position.x, position.y);
            temp.CreateOnGrid(AttachToGrid);
            return temp;
        }

        public void CreateOnGrid(Grid newGrid)
        {
            attachedGrid = newGrid;
            attachedGrid.AddNewGameObject(this);
        }

        //Set world to true if you want to attempt moving to a position on an overlapping grid.
        //Only returns false if World is true and position is not available.
        public bool MoveToGrid(Grid newGrid, bool World)
        {
            if (World)
            {
                if (!newGrid.PosIsAvailable((position + attachedGrid.worldPosition) - newGrid.worldPosition, true))
                    return false;

                position = (position - newGrid.worldPosition) + attachedGrid.worldPosition;
            }
            if (attachedGrid != null)
            {
                if (player)
                {
                    attachedGrid.ClearActiveObjects();
                    newGrid.ClearActiveObjects();
                }
                attachedGrid.RemoveGameObject(this);
            }
            attachedGrid = newGrid;
            attachedGrid.MoveExistingGameObjectHere(this);
            return true;
        }
        
        public GameObject()
        {
            sign = new Sign();
        }

        public GameObject(Grid AttachToGrid) 
        {
            attachedGrid = AttachToGrid;
            attachedGrid.AddNewGameObject(this);
        }

        public virtual void Move(int X, int Y)
        {
            if (attachedGrid.PosIsAvailable(new Position(position.x + X, position.y + Y), false))
            {
                position = new Position(position.x + X, position.y + Y);
            }
        }
    }
}
