using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overloadingtut
{
    class Portal : GameObject, IUsable
    {
        public GameObject linkedObject;
        public void Use(GameObject User)
        {
            if (linkedObject == null)
                return;

            if (linkedObject.attachedGrid.PosIsAvailable(linkedObject.position, true))
            {
                User.position = linkedObject.position;
                User.MoveToGrid(linkedObject.attachedGrid, false);
            }
        }

        public Portal()
        {
            sign.sign = 'P';
            sign.color = ConsoleColor.Red;
            enterable = true;
        }
    }
}
