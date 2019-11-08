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
        public double GridSize => this._gridSize;
        public int IterationCount = 0;
        public bool IsActive
        {
            get
            {
                foreach (var agent in Agents)
                {
                    CellGroup cellGroup = agent as CellGroup;
                    if (cellGroup.IsActive) return true;
                }
                return false;
            }
        }

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
                    foreach (var cell in cellGroup.Cells)
                        cellCenters.Add(cellGroup.GetPose(cell.X, cell.Y), pth);
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
                        cells.Add(GetCellFromPose(this.CellPoses.Branch(i)[j]), pth);
                    }
                }
                return cells;
            }
        }

        public DataTree<Point3d> PlaceToConnect
        {
            get
            {
                DataTree<Point3d> places = new DataTree<Point3d>();
                for (int i = 0; i < this.Agents.Count; i++)
                {
                    GH_Path pth = new GH_Path(i);
                    CellGroup cellGroup = this.Agents[i] as CellGroup;
                    places.AddRange(cellGroup.PlaceToConnect(), pth);
                }

                return places;
            }
        }

        public DataTree<bool> State
        {
            get
            {
                DataTree<bool> isActive = new DataTree<bool>();
                int i = 0;
                foreach (var agent in this.Agents)
                {
                    GH_Path pth = new GH_Path(i);
                    CellGroup cellGroup = agent as CellGroup;
                    foreach (var cell in cellGroup.Cells)
                    {
                        isActive.Add(cell.IsActive, pth);
                    }
                    i += 1;
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
