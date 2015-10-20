using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overloadingtut
{
    class MultiDimensionalBeam
    {
        private List<Grid> grids;
        private List<BeamObject> beams = new List<BeamObject>();
        private Position pos;

        //Supply a list of grids and it will create BeamObjects that stay in the same position in the world across all selected grids
        public MultiDimensionalBeam (List<Grid> Grids, Position WorldPosition, int Count, int Radius)
        {
            int offset = Radius;
            pos = WorldPosition;
            grids = Grids;
            Random rnd = new Random();
            for (int i = 0; i < Count; i++)
            {
                int dis = rnd.Next(offset);
                double rnddeg = rnd.Next(3600) / 10d;
                double tx = (dis * Math.Cos(rnddeg)) - (dis * Math.Sin(rnddeg));
                double ty = (dis * Math.Sin(rnddeg)) + (dis * Math.Cos(rnddeg));
                foreach (Grid g in grids)
                {
                    Position temppos = new Position((int) Math.Round(tx), (int) Math.Round(ty));
                    BeamObject b = GameObject.Instantiate<BeamObject>(pos, g);
                    b.worldPos = g.worldPosition;
                    b.Offset = temppos;
                    beams.Add(b);
                }
            }
        }

        private double curdeg = 0.0;
        private int updateActiveCounter = 0;
        private const int updateActiveFrequency = 20;

        //Call this every frame to make sure everything spins in the circle it was meant to spin
        public void UpdateBeams()
        {
            updateActiveCounter += 1;
            foreach (BeamObject b in beams)
            {
                int disx = b.Offset.x;
                int disy = b.Offset.y;
                double tx = (disx * Math.Cos(curdeg)) - (disy * Math.Sin(curdeg));
                double ty = (disx * Math.Sin(curdeg)) + (disy * Math.Cos(curdeg));
                b.Position = new Position((int) Math.Round(tx), (int) Math.Round(ty)) + pos - b.worldPos;
                if (updateActiveCounter >= updateActiveFrequency) b.attachedGrid.IsInRangeOfLastActiveCheck(b);
            }
            if (updateActiveCounter >= updateActiveFrequency) updateActiveCounter = 0;
            curdeg += 0.05;
            if (curdeg >= 360) curdeg -= 360;
        }
    }
}
