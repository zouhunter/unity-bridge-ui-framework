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
using NodeGraph.DataModel;
using NodeGraph;
using System;
using BridgeUI.Model;
using BridgeUI;

[CustomNode("Panel", 1,"BridgeUI")]
public class PanelNode : PanelNodeBase
{
    public PanelNode() { }
    public override string NodeOutputType
    {
        get
        {
            return "BridgeUI";
        }
    }
    public override string NodeInputType
    {
        get
        {
            return "BridgeUI";
        }
    }
    public PanelNode(string prefabPath)
    {
#if UNITY_EDITOR
        Info.prefabGuid = UnityEditor. AssetDatabase.AssetPathToGUID(prefabPath);
#endif
    }
    public override void Initialize(NodeData data)
    {
        data.AddDefaultOutputPoint();
        data.AddDefaultInputPoint();
    }

}
