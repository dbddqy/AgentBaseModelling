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
        //public override void Execute(AgentBase agent)
        //{
        //    CellGroup cellGroup = agent as CellGroup;
        //    Vector3d t = new Vector3d(rd.NextDouble()-0.5, rd.NextDouble()-0.5, 0.0);
        //    double r = rd.NextDouble() * Math.PI * 0.05;
        //    cellGroup.Move(t, r);
        //}

        public void ExecutWithSystem(CellGroup cellGroup, CellAgentSystem cellSystem)
        {
            Vector3d t = new Vector3d(rd.NextDouble() - 0.5, rd.NextDouble() - 0.5, 0.0);
            double r = (rd.NextDouble() - 0.5) * Math.PI * 0.05;
            t += FindNeighor(cellGroup, cellSystem);
            cellGroup.Move(t, r); 
        }

        private Vector3d FindNeighor(CellGroup cellGroup, CellAgentSystem cellSystem)
        {
            int minIndex = -1;
            double minDis = 999999999999999999;
            for (int i = 0; i < cellSystem.Agents.Count; i++)
            {
                CellGroup g = cellSystem.Agents[i] as CellGroup;
                if (cellGroup.Pose.Origin.DistanceTo(g.Pose.Origin) < 0.01) continue;
                if (minDis> cellGroup.Pose.Origin.DistanceTo(g.Pose.Origin))
                {
                    minIndex = i;
                    minDis = cellGroup.Pose.Origin.DistanceTo(g.Pose.Origin);
                }
            }
            CellGroup gMin = cellSystem.Agents[minIndex] as CellGroup;
            Vector3d v = new Vector3d(gMin.Pose.Origin - cellGroup.Pose.Origin);
            v.Unitize();
            return v * 0.8;
        }
    }
}
