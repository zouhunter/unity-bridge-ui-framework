using UnityEngine;
using NodeGraph.DataModel;
using BridgeUI.Model;
using BridgeUI.CodeGen;
using System.Collections.Generic;

public abstract class PanelNodeBase : Node, IPanelInfoHolder
{
    public int selected;
    public int instenceID;
    public string description;
    public string assetName;
    public int style = 1;
    //public NodeType nodeType = NodeType.Destroy | NodeType.Fixed | NodeType.HideGO | NodeType.NoAnim | NodeType.ZeroLayer;
    public GenCodeRule rule;
    public List<ComponentItem> components = new List<ComponentItem>();
    public NodeInfo nodeInfo = new NodeInfo();
    public NodeInfo Info
    {
        get
        {
            return nodeInfo;
        }
    }
}