using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;
using NodeGraph.DataModel;
using NodeGraph;

[CustomNodeGraphDrawer(typeof(ObjectNode))]
public class PanelNodeDrawer : NodeDrawer
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
            return "panel";
        }
    }
    public override float CustomNodeHeight
    {
        get
        {
            return EditorGUIUtility.singleLineHeight * 1;
        }
    }
    public override void OnNodeGUI(Rect position)
    {
        base.OnNodeGUI(position);
        (target as ObjectNode).type =  (PrimitiveType)EditorGUI.EnumPopup(position,(target as ObjectNode).type);
    }
    public override void OnInspectorGUI(NodeGUI gui)
    {
        base.OnInspectorGUI(gui);
        gui.Name = (target as ObjectNode).type.ToString();
    }

}