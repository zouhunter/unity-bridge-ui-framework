using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NodeGraph.DataModel;
using NodeGraph;

[CustomNode("RemotePanel", 2, "BridgeUI")]
public class RemoteNode : Node {
    protected override IEnumerable<Point> inPoints
    {
        get
        {
            return new Point[] { new Point("", "bridge", 100) };
        }
    }
}
