using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using NodeGraph.DataModel;
using NodeGraph;

[CustomNode("Luncher", 0)]
public class Luncher : Node
{
    public int a;
    public override string NodeOutputType
    {
        get
        {
            return "BridgeUI";
        }
    }
  
    public override void Initialize(NodeData data)
    {
        data.AddDefaultOutputPoint();
        Debug.Log("Initialize");
    }
}