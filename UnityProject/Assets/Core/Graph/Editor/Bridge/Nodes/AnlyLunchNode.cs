using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;
using V1 = AssetBundleGraph;
using Model = NodeGraph.DataModel.Version2;
using NodeGraph;

[CustomNode("Lunch/AnyState", 3)]
public class AnyLunchNode : Node
{
    public override string ActiveStyle
    {
        get
        {
            return "node 3 on";
        }
    }

    public override string InactiveStyle
    {
        get
        {
            return "node 3";
        }
    }

    public override string Category
    {
        get
        {
            return "anly";
        }
    }
    public override void Initialize(Model.NodeData data)
    {
        data.AddDefaultOutputPoint();
        Debug.Log("Initialize");
    }

    public override Node Clone(Model.NodeData newData)
    {
        var newNode = new EnableLunchNode();
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