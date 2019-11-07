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
        public bool IsActive 
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
                return (numPassive <= (numActive * _ratioActivePassive));
            }
        }

        public List<Point3d> Positions
        {
            get
            {
                List<Point3d> positions = new List<Point3d>();
                foreach (Cell cell in this.Cells) positions.Add(cell.GetPosition(this.Pose));
                return positions;
            }
        }

        private int _ratioActivePassive = 7;

        #endregion


        #region constructors

        public CellGroup() 
        {
            this.Cells = new List<Cell>();
            base.Behaviors = new List<BehaviorBase>();
        }
        public CellGroup(Cell cell) : this() { this.Cells.Add(cell); }
        public CellGroup(List<Cell> cells) : this() { this.Cells = cells; }
        public CellGroup(Cell cell, BehaviorBase behavior) : this(cell) { this.Behaviors.Add(behavior); }
        public CellGroup(List<Cell> cells, BehaviorBase behavior) : this(cells) { this.Behaviors.Add(behavior); }

        #endregion

        internal List<Plane> GetPoses(double gridSize)
        {
            Vector3d x1 = new Vector3d(gridSize, 0.0, 0.0);
            Vector3d x2 = new Vector3d(gridSize * 0.5, gridSize * 0.5 / Math.Sqrt(3), 0.0);
            Vector3d y = new Vector3d(gridSize * 0.5, gridSize * 0.5 * Math.Sqrt(3), 0.0);
            List<Plane> planes = new List<Plane>();
            Vector3d t;
            double r;
            foreach (var cell in this.Cells)
            {
                t = (x1 * (cell.X / 2)) + (x2 * (cell.X % 2)) + (y * cell.Y);
                r = (cell.X % 2 == 0) ? 0.0 : Math.PI;
                Plane pose = this.Pose.Clone();
                pose.Translate(t);
                pose.Rotate(r, pose.ZAxis);
                planes.Add(pose);
            }
            return planes;
        }

        internal void Move(Vector3d t, Double r)
        {
            if (!this.IsActive) return;
            this.Pose.Translate(t);
            this.Pose.Rotate(r, this.Pose.ZAxis);
        }

        internal void Merge(CellGroup group)
        {

        }

        #region override functions

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
