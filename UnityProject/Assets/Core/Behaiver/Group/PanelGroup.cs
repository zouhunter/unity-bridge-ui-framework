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
using System.Collections.Generic;
using System;
/// <summary>
/// 用于标记ui打开的父级
/// [3维场景中可能有多个地方需要打开用户界面]
/// </summary>
public abstract class PanelGroup : MonoBehaviour , IPanelGroup
{
    public List<BridgeObj> bridges;

    public abstract List<UINodeBase> Nodes { get; }

    public abstract List<PanelGroupObj> SubGroups { get; }

    public Transform Trans { get { return transform; } }

    public bool Contains(string panelName)
    {
        return Nodes.Find(x => x.panelName == panelName) != null;
    }

    public BridgeObj OpenPanel(IPanelBase parentPanel, string panelName, object data)
    {
        var inpanelName = parentPanel == null ? "" : parentPanel.Name;
        var bridge = bridges.Find(x => x.outNode == panelName && x.inNode == inpanelName);
        if(bridge != null)
        {
            var bg = Instantiate(bridge);
            bg.QueueSend(data);
            return bg;
        }
        return null;
    }

    protected virtual void Awake()
    {
        UIFacade.RegistGroup(this);
    }
    protected virtual void OnDestroy()
    {
        UIFacade.UnRegistGroup(this);
    }
}
