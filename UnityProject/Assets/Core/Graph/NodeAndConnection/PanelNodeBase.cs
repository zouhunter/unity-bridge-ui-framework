using UnityEngine;
using UnityEngine.Events;
using NodeGraph.DataModel;
using NodeGraph;
using System;
using BridgeUI.Model;
using BridgeUI;

public abstract class PanelNodeBase : Node, IPanelInfoHolder
{
    public int instenceID;
    public string description;
    public string assetName;
    public int style = 1;
    public NodeType nodeType = NodeType.Destroy | NodeType.Fixed | NodeType.HideGO | NodeType.NoAnim | NodeType.ZeroLayer;

    public NodeInfo nodeInfo = new NodeInfo();
    public NodeInfo Info
    {
        get
        {
            return nodeInfo;
        }
    }
}