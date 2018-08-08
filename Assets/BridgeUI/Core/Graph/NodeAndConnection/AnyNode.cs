using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using NodeGraph.DataModel;
using NodeGraph;
namespace BridgeUI.Graph
{
    [CustomNode("a.AnyPanel", 0, "BridgeUI")]
    public class AnyNode : Node
    {
        public override void Initialize(NodeData data)
        {
            base.Initialize(data);
            if (data.OutputPoints == null || data.OutputPoints.Count == 0)
            {
                data.AddOutputPoint("→", "bridge", 100);
            }
        }
    }
}