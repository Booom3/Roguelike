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

        public static T Instantiate<T>(Position position, Grid AttachToGrid) where T : GameObject
        {
            T temp = (T)Activator.CreateInstance(typeof(T));
            temp.position = new Position(position.x, position.y);
            temp.MoveToGrid(AttachToGrid);
            return temp;
        }

        public void MoveToGrid(Grid newGrid)
        {
            attachedGrid = newGrid;
            attachedGrid.AddGameObject(this);
        }
        
        public GameObject()
        {
            sign = new Sign();
        }

        public GameObject(Grid AttachToGrid) 
        {
            attachedGrid = AttachToGrid;
            attachedGrid.AddGameObject(this);
        }

        public virtual void Move(int X, int Y)
        {
            if (attachedGrid.PosIsAvailable(new Position(position.x + X, position.y + Y)))
            {
                position = new Position(position.x + X, position.y + Y);
            }
        }
    }
}
