using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;
using V1 = AssetBundleGraph;
using Model = NodeGraph.DataModel.Version2;
using NodeGraph;

[CustomNode("Lunch/Enable", 2)]
public class EnableLunchNode : Node
{
    public override string ActiveStyle
    {
        get
        {
            return "node 2 on";
        }
    }

    public override string InactiveStyle
    {
        get
        {
            return "node 2";
        }
    }

    public override string Category
    {
        get
        {
            return "enable";
        }
    }
    private Model.NodeData data;
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
        data = node.Data;
        EditorGUILayout.HelpBox("Split By Filter: Split incoming assets by filter conditions.", MessageType.Info);
        editor.UpdateNodeName(node);
    }
    public override void OnContextMenuGUI(GenericMenu menu)
    {
        base.OnContextMenuGUI(menu);
        menu.AddItem(new GUIContent("Add Node"), false, () =>
        {
            Debug.Log("Add");
            data.AddOutputPoint("");
        });
        menu.AddItem(new GUIContent("Delete Node"), false, () =>
        {
            Debug.Log("Delete");

        });
    }
}