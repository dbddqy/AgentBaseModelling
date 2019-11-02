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
    public class MyAgent : MeshFaceAgent
    {
        public int myDefaultValue;

        public MyAgent(int faceIndex, double initialValue, List<BehaviorBase> behaviors) : base(faceIndex, initialValue, behaviors)
        {
            this.myDefaultValue = 1;
        }


    }
}
