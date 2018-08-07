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
using NodeGraph.DataModel;
using NodeGraph;
using System;
using BridgeUI.Model;
using BridgeUI;
using System.Collections.Generic;

[CustomNode("b.RealPanel", 1, "BridgeUI")]
public class PanelNode : PanelNodeBase
{
    public override void Initialize(NodeData data)
    {
        base.Initialize(data);

        if (data.InputPoints == null || data.InputPoints.Count == 0)
        {
            data.AddInputPoint("→", "bridge", 100);
        }

        if (data.OutputPoints == null || data.OutputPoints.Count == 0)
        {
            data.AddOutputPoint("0", "bridge", 100);
        }

        if (data.Object != null && data.Object is PanelNode)
        {
            var panelNode = data.Object as PanelNode;
            var panel = GetPanelFromGUID(panelNode.Info.guid);
            if (panel != null && panel.Capacity != data.OutputPoints.Count)
            {
                if (panel.Capacity > data.OutputPoints.Count)
                {
                    for (int i = 0; i < panel.Capacity; i++)
                    {
                        if (data.OutputPoints.Count <= i)
                        {
                            data.AddOutputPoint(i.ToString(), "bridge", 100);
                        }
                    }
                }
                else 
                {
                    var more = data.OutputPoints.Count - panel.Capacity;
                    for (int i = 0; i < more; i++)
                    {
                        data.OutputPoints.RemoveAt(data.OutputPoints.Count - 1);
                    }
                }
            }
        }
    }

    private IUIPanel GetPanelFromGUID(string guid)
    {
#if UNITY_EDITOR
        var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        if (!string.IsNullOrEmpty(path))
        {
            var go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return go.GetComponent<IUIPanel>();
        }
#endif
        return null;
    }
}
