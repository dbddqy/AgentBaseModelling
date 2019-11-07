using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

namespace AgentBasedModelling
{
    public struct Cell
    {
        public int X;
        public int Y;
        public bool IsActive;

        public Cell(int x, int y, bool isActive)
        {
            X = x;
            Y = y;
            IsActive = isActive;
        }

        internal Point3d GetPosition(Plane pose)
        {
            throw new NotImplementedException();
        }
    }
}
