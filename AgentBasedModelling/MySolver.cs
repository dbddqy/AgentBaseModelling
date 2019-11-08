using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ICD.AbmFramework.Core.Agent;

namespace AgentBasedModelling
{
    public class MySolver
    {
        public CellAgentSystem System;

        public MySolver(CellAgentSystem system)
        {
            this.System = system;
        }

        public void ExecuteSingleStep()
        {
            if (System.Agents.Count == 1) return;
            foreach (AgentBase agent in System.Agents)
            {
                CellGroup cellGroup = agent as CellGroup;
                cellGroup.ExecuteWithSystem(System);
                System.IterationCount += 1;
            }
        }
    }
}
