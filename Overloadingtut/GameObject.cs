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
        public Position velocity;
        public bool bounceenabled = true;
        private int nextmove = 0;
        private int nextgravity = 0;

        public static T Instantiate<T>(Position position, Grid AttachToGrid) where T : GameObject
        {
            T temp = (T)Activator.CreateInstance(typeof(T));
            temp.position = new Position(position.x, position.y);
            temp.MoveToGrid(AttachToGrid);
            return temp;
        }

        public void Update()
        {
            if (bounceenabled)
            {
                if (nextmove > 5)
                {
                    if (velocity.y > -1)
                    {
                        if (nextgravity > 5)
                        {
                            nextgravity = 0;
                            velocity.y--;
                        }
                        else
                            nextgravity++;
                    }
                    if (attachedGrid.PosIsAvailable(new Position(position.x + velocity.x, position.y)))
                    {
                        position.x += velocity.x;
                    }
                    else
                    {
                        velocity.x = -velocity.x;
                    }
                    if (attachedGrid.PosIsAvailable(new Position(position.x, position.y + velocity.y)))
                    {
                        position.y += velocity.y;
                    }
                    else
                    {
                        velocity.y = -velocity.y;
                    }
                    nextmove = 0;
                }
                else
                    nextmove++;
            }
        }

        public void MoveToGrid(Grid newGrid)
        {
            attachedGrid = newGrid;
            attachedGrid.AddGameObject(this);
        }
        
        public GameObject()
        {
            sign = new Sign();
            velocity = new Position(new Random().Next(-1, 2), -1);
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
