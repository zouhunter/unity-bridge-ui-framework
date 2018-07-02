using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeGraph.DataModel
{
    /// <summary>
    /// Node.
    /// </summary>
	public abstract class Node:ScriptableObject{
        public virtual void Initialize(NodeData data)
        {
            InitInputPoints(data);
            InitOutPoints(data);
        }
        protected void InitInputPoints(NodeData data)
        {
            if (inPoints != null)
            {
                var updated = new List<ConnectionPointData>();
                foreach (var point in inPoints)
                {
                    var old = data.InputPoints.Find(x => x.Label == point.label && !updated.Contains(x));
                    if (old != null)
                    {
                        updated.Add(old);
                        old.Max = point.max;
                        old.Type = point.type;
                    }
                    else
                    {
                        old = data.AddInputPoint(point.label, point.type, point.max);
                        updated.Add(old);
                    }
                }
                data.InputPoints.Clear();
                data.InputPoints.AddRange(updated);
            }
        }
        protected void InitOutPoints(NodeData data)
        {
            if (outPoints != null)
            {
                var updated = new List<ConnectionPointData>();
                foreach (var point in outPoints)
                {
                    var old = data.OutputPoints.Find(x => (x.Label == point.label /*|| x.Type == point.type*/) && !updated.Contains(x));
                    if (old != null)
                    {
                        updated.Add(old);
                        old.Label = point.label;
                        old.Max = point.max;
                        old.Type = point.type;
                    }
                    else
                    {
                        old = data.AddOutputPoint(point.label, point.type, point.max);
                        updated.Add(old);
                    }
                }
                data.OutputPoints.Clear();
                data.OutputPoints.AddRange(updated);
            }
        }
        protected virtual IEnumerable<Point> inPoints { get { return null; } }
        protected virtual IEnumerable<Point> outPoints { get{ return null; } }
    }

}
