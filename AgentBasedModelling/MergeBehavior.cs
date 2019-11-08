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
    public class MergeBehavior : BehaviorBase
    {
        //public override void Execute(AgentBase agent)
        //{
        //    CellGroup cellGroup = agent as CellGroup;
        //    if (!cellGroup.IsActive) return;
        //    CellAgentSystem cellSystem = cellGroup.AgentSystem as CellAgentSystem;

        //    for (int i = 0; i < cellSystem.Agents.Count; i++)
        //    {
        //        CellGroup anotherGroup = cellSystem.Agents[i] as CellGroup;
        //        if (cellGroup == anotherGroup) continue;
        //        // check if the agent can merge with another agnent
        //        foreach (Point3d place in cellGroup.PlaceToConnect())
        //        {
        //            foreach (Plane pose in cellSystem.CellPoses.Branch(i))
        //            {
        //                if (place.DistanceTo(pose.Origin) < 0.75 * cellSystem.GridSize)
        //                {
        //                    cellGroup.Merge(anotherGroup, place, pose);
        //                    cellSystem.Agents.RemoveAt(i);
        //                    goto NextStep;
        //                }
        //            }
        //        }
        //    NextStep: continue;
        //    }
        //}

        public void ExecuteWithSystem(CellGroup cellGroup, CellAgentSystem cellSystem)
        {
            if (!cellGroup.IsActive) return;

            //List<int> RemovedIndices = new List<int>(); //for the bug during merging
            for (int i = 0; i < cellSystem.Agents.Count; i++)
            {
                //if (RemovedIndices.Contains(i)) continue; //for the bug during merging
                CellGroup anotherGroup = cellSystem.Agents[i] as CellGroup;
                if (cellGroup == anotherGroup) continue;
                // check if the agent can merge with another agnent
                foreach (Point3d place in cellGroup.PlaceToConnect())
                {
                    foreach (Plane pose in cellSystem.CellPoses.Branch(i))
                    {
                        if (place.DistanceTo(pose.Origin) < 0.75 * cellSystem.GridSize)
                        {
                            cellGroup.Merge(anotherGroup, place, pose);
                            cellSystem.Agents.RemoveAt(i);
                            //RemovedIndices.Add(i); //for the bug during merging
                            goto NextStep;
                        }
                    }
                }
            NextStep: continue;
            }
        }
    }
}
