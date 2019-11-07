using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;
using ICD.AbmFramework.Core.Agent;
using ICD.AbmFramework.Core.Behavior;

namespace AgentBasedModelling
{
    public class WanderBehavior : BehaviorBase
    {
        private Random rd = new Random();
        public override void Execute(AgentBase agent)
        {
            CellGroup cellGroup = agent as CellGroup;
            Vector3d t = new Vector3d(rd.NextDouble()-0.5, rd.NextDouble()-0.5, 0.0);
            double r = rd.NextDouble() * Math.PI * 0.5;
            cellGroup.Move(t, r);
        }
    }
}
