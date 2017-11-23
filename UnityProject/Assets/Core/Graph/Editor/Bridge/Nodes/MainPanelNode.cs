using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;
using V1 = AssetBundleGraph;
using Model = NodeGraph.DataModel.Version2;
using NodeGraph;
[CustomNode("Panel/MainPanel", 1)]
public class MainPanelNode : PanelNodeBase
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

    protected override string HeadInfo
    {
        get
        {
            return "this is one main panel of scene,hold other panel ,and can open by defult rule";
        }
    }

    public override void Initialize(Model.NodeData data)
    {
        data.AddDefaultOutputPoint();
        Debug.Log("Initialize");
    }

    public override void OnContextMenuGUI(GenericMenu menu)
    {
        base.OnContextMenuGUI(menu);
    }
}