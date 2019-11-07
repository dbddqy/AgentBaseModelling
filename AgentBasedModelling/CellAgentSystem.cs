using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;
using ICD.AbmFramework.Core.AgentSystem;

namespace AgentBasedModelling
{
    public class CellAgentSystem : AgentSystemBase
    {
        private double _gridSize;
        public double GridSize { get { return this._gridSize; } }

        #region visualization
        public DataTree<Plane> CellPoses
        {
            get
            {
                DataTree<Plane> cellCenters = new DataTree<Plane>();
                for (int i = 0; i < this.Agents.Count; i++)
                {
                    GH_Path pth = new GH_Path(i);
                    CellGroup cellGroup = this.Agents[i] as CellGroup;
                    cellCenters.AddRange(cellGroup.GetPoses(this.GridSize), pth);
                }
                return cellCenters;
            }
        }

        public DataTree<PolylineCurve> Cells 
        { 
            get
            {
                DataTree<PolylineCurve> cells = new DataTree<PolylineCurve>();
                for (int i = 0; i < this.CellPoses.BranchCount; i++)
                {
                    GH_Path pth = new GH_Path(i);
                    for (int j = 0; j < this.CellPoses.Branch(i).Count; j++)
                    {
                        cells.Add(GetCellFromPose(this.CellPoses.Branch(i)[j]));
                    }
                }
                return cells;
            } 
        }

        public DataTree<bool> IsActive
        {
            get
            {
                DataTree<bool> isActive = new DataTree<bool>();
                foreach (var agent in this.Agents)
                {
                    GH_Path pth = new GH_Path();
                    CellGroup cellGroup = agent as CellGroup;
                    foreach (var cell in cellGroup.Cells)
                    {
                        isActive.Add(cell.IsActive);
                    }
                }
                return isActive;
            }
        }

        private PolylineCurve GetCellFromPose(Plane plane)
        {
            List<Point3d> ps = new List<Point3d>();
            ps.Add(plane.PointAt(0.0, this.GridSize / Math.Sqrt(3)));
            ps.Add(plane.PointAt(-this.GridSize * 0.5, -this.GridSize * 0.5 / Math.Sqrt(3)));
            ps.Add(plane.PointAt(this.GridSize * 0.5, -this.GridSize * 0.5 / Math.Sqrt(3)));
            ps.Add(plane.PointAt(0.0, this.GridSize / Math.Sqrt(3)));
            return new PolylineCurve(ps);
        }

        #endregion

        public CellAgentSystem(double size) { this._gridSize = size; }
    }
}
