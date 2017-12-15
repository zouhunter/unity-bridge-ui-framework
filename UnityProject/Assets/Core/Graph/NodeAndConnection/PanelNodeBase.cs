using UnityEngine;
using UnityEngine.Events;
using NodeGraph.DataModel;
using NodeGraph;
using System;
using UnityEditor;
using BridgeUI.Model;
using BridgeUI;

public abstract class PanelNodeBase : Node, IPanelInfoHolder
{
    public NodeInfo nodeInfo = new NodeInfo();
    public NodeInfo Info
    {
        get
        {
            return nodeInfo;
        }
    }
}