using UnityEngine;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

using V1 = AssetBundleGraph;
using Model = NodeGraph.DataModel.Version2;
using NodeGraph;

[CustomNode("UINode/PrefabUINode", 1)]
public class PrefabUINode : Node
{

    public override string ActiveStyle
    {
        get
        {
            return "node 0 on";
        }
    }

    public override string InactiveStyle
    {
        get
        {
            return "node 0";
        }
    }

    public override string Category
    {
        get
        {
            return "prefab";
        }
    }

    public PrefabUIInfo uiInfo;

    public PrefabUINode() { }

    public override void Initialize(Model.NodeData data)
    {
        data.AddDefaultInputPoint();
        data.AddDefaultOutputPoint();
    }

    public override Node Clone(Model.NodeData newData)
    {
        var newNode = new PrefabUINode();
        newData.AddDefaultOutputPoint();
        return newNode;
    }

    public override void OnInspectorGUI(NodeGUI node,  NodeGUIEditor editor, Action onValueChanged)
    {
        EditorGUILayout.TextField(uiInfo.panelName);
    }
}
