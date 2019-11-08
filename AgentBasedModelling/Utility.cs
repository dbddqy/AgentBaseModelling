using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentBasedModelling
{
    class Utility
    {
        public static void Mod(int x, int y, out int q, out int r)
        {
            q = (int)Math.Floor((double)x / y);
            r = x - (q * y);
        }
    }
}
