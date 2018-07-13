using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using NodeGraph.DataModel;
using NodeGraph;

[CustomNode("AnyState", 0, "BridgeUI")]
public class Luncher : Node
{
    protected override IEnumerable<Point> outPoints
    {
        get
        {
            return new Point[] { new Point("+", "bridge", 100) };
        }
    }
}