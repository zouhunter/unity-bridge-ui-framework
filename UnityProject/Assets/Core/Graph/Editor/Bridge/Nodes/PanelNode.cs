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


[CustomNode("Panel/PanelNode", 5)]
public class PanelNode : PanelNodeBase
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
    protected override string HeadInfo
    {
        get
        {
            return "Panel Node : record panel load type and other rule";
        }
    }
    public override void Initialize(Model.NodeData data)
    {
        data.AddDefaultOutputPoint();
        data.AddDefaultInputPoint();
    }


    public override void OnContextMenuGUI(GenericMenu menu)
    {
        base.OnContextMenuGUI(menu);
    }
}
