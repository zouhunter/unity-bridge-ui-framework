using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;
using System;

[CustomNode("RundomConnect/Node", 1)]
public class ObjectNode : Node {
    public PrimitiveType type;
    public override string NodeInputType
    {
        get
        {
            return Line_connection.type;
        }
    }
    public override string NodeOutputType
    {
        get
        {
            return Line_connection.type;
        }
    }
    public override void Initialize(NodeData data)
    {
        if (data.InputPoints.Find(x => x.Label == "a") == null) data.AddInputPoint("a");
        if (data.OutputPoints.Find(x => x.Label == "b") == null) data.AddOutputPoint("b");
    }
}
