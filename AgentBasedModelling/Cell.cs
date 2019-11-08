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

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
            IsActive = false;
        }
        public Cell(int x, int y, bool isActive) : this(x, y) { IsActive = isActive; }

        public List<Cell> Neighbors
        {
            get
            {
                List<Cell> Neighbors = new List<Cell>();
                Neighbors.Add(new Cell(X - 1, Y));
                Neighbors.Add(new Cell(X + 1, Y));
                if (X % 2 == 0)
                    Neighbors.Add(new Cell(X + 1, Y - 1));
                else
                    Neighbors.Add(new Cell(X - 1, Y + 1));
                return Neighbors;
            }
        }

        public static bool operator ==(Cell a, Cell b) => (a.X == b.X) && (a.Y == b.Y);
        public static bool operator !=(Cell a, Cell b) => (a.X != b.X) || (a.Y != b.Y);
    }
}
