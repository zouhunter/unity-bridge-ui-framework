using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;
using V1 = AssetBundleGraph;
using Model = NodeGraph.DataModel.Version2;
using NodeGraph;
[CustomNode("Panel/MainPanel", 1)]
public class MainLunchNode : Node
{
    public override string ActiveStyle
    {
        get
        {
            return "node 1 on";
        }
    }

    public override string InactiveStyle
    {
        get
        {
            return "node 1";
        }
    }

    public override string Category
    {
        get
        {
            return "main";
        }
    }
    public override void Initialize(Model.NodeData data)
    {
        data.AddDefaultOutputPoint();
        Debug.Log("Initialize");
    }

    public override Node Clone(Model.NodeData newData)
    {
        var newNode = new MainLunchNode();
        Debug.Log("Clone");
        return newNode;
    }

    public override void OnInspectorGUI(NodeGUI node, NodeGUIEditor editor, Action onValueChanged)
    {
        EditorGUILayout.HelpBox("Main Panel: Defult Open Rule,And Hold Other Panels", MessageType.Info);
        editor.UpdateNodeName(node);
    }
    public override void OnContextMenuGUI(GenericMenu menu)
    {
        base.OnContextMenuGUI(menu);
    }
}