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
    public class CellGroup : AgentBase
    {
        #region fields and properties

        public List<Cell> Cells;
        public Plane Pose;
        public bool IsActive => this.Speed > 0;

        public double Speed
        {
            get
            {
                int numActive = 0, numPassive = 0;
                foreach (Cell cell in this.Cells)
                {
                    if (cell.IsActive)
                        numActive += 1;
                    else
                        numPassive += 1;
                }
                return ((double)numActive / (numActive + numPassive) - (1.0 / (_ratioActivePassive + 1)));// 1 can carry 8(include itself)
            }
        }

        private int _ratioActivePassive = 7;

        private Vector3d DirX1 => this.Pose.XAxis;

        private Vector3d DirX2 => 0.5 * this.Pose.XAxis + 0.5 / Math.Sqrt(3) * this.Pose.YAxis;
        private Vector3d DirY => 0.5 * this.Pose.XAxis + 0.5 * Math.Sqrt(3) * this.Pose.YAxis;

        private double _gridSize;
        public double GridSize => this._gridSize;

        internal bool toDelete = false;

        #endregion


        #region constructors

        public CellGroup(double gridSize) 
        {
            this.Cells = new List<Cell>();
            base.Behaviors = new List<BehaviorBase>();
            this._gridSize = gridSize;
        }
        public CellGroup(Cell cell, double gridSize) : this(gridSize) { this.Cells.Add(cell); }
        public CellGroup(List<Cell> cells, double gridSize) : this(gridSize) { this.Cells = cells; }
        public CellGroup(Cell cell, BehaviorBase behavior, double gridSize) : this(cell, gridSize) { this.Behaviors.Add(behavior); }
        public CellGroup(List<Cell> cells, BehaviorBase behavior, double gridSize) : this(cells, gridSize) { this.Behaviors.Add(behavior); }

        #endregion

        internal List<Point3d> PlaceToConnect()
        {
            List<Point3d> places = new List<Point3d>();
            for (int i = 0; i < Cells.Count; i++)
            {
                foreach (Cell cell in Cells[i].Neighbors)
                {
                    if (isExisting(cell)) continue;
                    places.Add(GetPose(cell.X, cell.Y).Origin);
                }
            }
            return places;
        }

        public Plane GetPose(int x, int y)
        {
            Utility.Mod(x, 2, out int q, out int r);
            Vector3d t = (DirX1 * q) + (DirX2 * r) + (DirY * y);

            t *= GridSize;
            double theta = (x % 2 == 0) ? 0.0 : Math.PI;

            Plane pose = new Plane(Pose);
            pose.Translate(t);
            pose.Rotate(theta, pose.ZAxis);

            return pose;
        }
        public Plane GetPose(Cell cell) => this.GetPose(cell.X, cell.Y);
        public void Move(Vector3d t, Double r)
        {
            if (!this.IsActive) return;
            this.Pose.Translate(t * this.Speed);
            this.Pose.Rotate(r * this.Speed, this.Pose.ZAxis);
        }

        public void SetActivePassiveRatio(int ratio) { this._ratioActivePassive = ratio; }
        internal void Merge(CellGroup anotherGroup, Point3d place, Plane anotherGroupPose)
        {
            Plane poseToConnect = GetPose(GetCellFromPoint(place));
            List<Cell> cellsToAdd = new List<Cell>();
            int times = 0;
            // try 3 different angles in which they can connect without collision
            while (true)
            {
                cellsToAdd.Clear();
                foreach (Cell cell in anotherGroup.Cells)
                {
                    Point3d cellCenter = anotherGroup.GetPose(cell).Origin;
                    Point3d cellCenterOriented = cellCenter;
                    cellCenterOriented.Transform(Transform.PlaneToPlane(anotherGroupPose, poseToConnect));
                    Cell cellToAdd = GetCellFromPoint(cellCenterOriented);
                    if (times < 3)
                        foreach (Cell c in Cells)
                            if (c == cellToAdd)
                                goto NextStep;
                    cellToAdd.IsActive = cell.IsActive;
                    cellsToAdd.Add(cellToAdd);
                }
                Cells.AddRange(cellsToAdd);
                break;
            NextStep:
                poseToConnect.Rotate(2 * Math.PI / 3, poseToConnect.ZAxis);
                times += 1;
            }
        }

        public Cell GetCellFromPoint(Point3d place)
        {
            double u, v;
            Pose.ClosestParameter(place, out u, out v); // might not be successful
            u /= GridSize;
            v /= GridSize;
            double a = u - (v / Math.Sqrt(3));
            double q, r, y;
            q = Math.Round(a);
            r = Math.Abs(a - q) < 0.1 ? 0 : 1;
            y = 2 * u - 2 * q - r;
            return new Cell((int)Math.Round(2*q+r), (int)Math.Round(y));
        }

        private bool isExisting(Cell cell)
        {
            foreach (Cell existingCell in this.Cells)
            {
                if (cell == existingCell)
                    return true;
            }
            return false;
        }

        #region override functions
        internal void ExecuteWithSystem(CellAgentSystem system)
        {
            WanderBehavior b0 = Behaviors[0] as WanderBehavior;
            b0.ExecutWithSystem(this, system);
            MergeBehavior b1 = Behaviors[1] as MergeBehavior;
            b1.ExecuteWithSystem(this, system);
        }
        public override void Execute()
        {
            foreach (var behavior in this.Behaviors)
            {
                behavior.Execute(this);
            }
        }
        public override void PostExecute() { }
        public override void PreExecute() { }
        public override void Reset() { }

        #endregion
    }
}
