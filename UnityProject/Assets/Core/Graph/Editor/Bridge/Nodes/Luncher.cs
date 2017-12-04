using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;
using V1 = AssetBundleGraph;
using Model = NodeGraph.DataModel.Version2;
using NodeGraph;

[CustomNode("Luncher", 0)]
public class Luncher : Node
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
            return "empty";
        }
    }
    public override void Initialize(Model.NodeData data)
    {
        data.AddDefaultOutputPoint();
        Debug.Log("Initialize");
    }

    public override Node Clone(Model.NodeData newData)
    {
        var newNode = new Luncher();
        Debug.Log("Clone");
        return newNode;
    }

    public override void OnInspectorGUI(NodeGUI node, NodeGUIEditor editor, Action onValueChanged)
    {
        EditorGUILayout.HelpBox("Any Lunch: Lunch SubPanels From Any State", MessageType.Info);
        editor.UpdateNodeName(node);
    }
    public override void OnContextMenuGUI(GenericMenu menu)
    {
        base.OnContextMenuGUI(menu);
    }
}