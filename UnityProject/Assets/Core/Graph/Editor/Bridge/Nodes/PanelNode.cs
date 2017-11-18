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
using V1 = AssetBundleGraph;
using Model = NodeGraph.DataModel.Version2;
using NodeGraph;
using System;
using UnityEditor;
public interface IPanelInfoHolder
{
    NodeInfo Info { get; }
}

[CustomNode("Panel/PanelNode", 5)]
public class PanelNode : Node, IPanelInfoHolder
{
    public override string ActiveStyle
    {
        get
        {
            return "node 5 on";
        }
    }

    public override string InactiveStyle
    {
        get
        {
            return "node 5";
        }
    }

    public override string Category
    {
        get
        {
            return "panel";
        }
    }

    public NodeInfo Info
    {
        get
        {
            return nodeInfo;
        }
    }

    public NodeInfo nodeInfo = new NodeInfo();

    public override void Initialize(Model.NodeData data)
    {
        data.AddDefaultOutputPoint();
        data.AddDefaultInputPoint();
        Debug.Log("Initialize");
    }

    public override Node Clone(Model.NodeData newData)
    {
        var newNode = new PanelNode();
        Debug.Log("Clone");
        return newNode;
    }

    public override void OnInspectorGUI(NodeGUI node, NodeGUIEditor editor, Action onValueChanged)
    {
        EditorGUILayout.HelpBox("Split By Filter: Split incoming assets by filter conditions.", MessageType.Info);
        editor.UpdateNodeName(node);
        nodeInfo.prefab = EditorGUILayout.ObjectField(nodeInfo.prefab, typeof(GameObject), false) as GameObject;
        if(nodeInfo.prefab != null) {
            node.Name = nodeInfo.prefab.name;
        }
    }
    public override void OnContextMenuGUI(GenericMenu menu)
    {
        base.OnContextMenuGUI(menu);
        
    }
}
