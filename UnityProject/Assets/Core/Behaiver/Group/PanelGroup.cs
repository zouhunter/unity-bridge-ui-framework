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
    private Dictionary<BridgeObj, BridgePool> poolDic = new Dictionary<BridgeObj, BridgePool>();
    public abstract List<UINodeBase> Nodes { get; }

    public abstract List<PanelGroupObj> SubGroups { get; }

    public Transform Trans { get { return transform; } }

    public bool Contains(string panelName)
    {
        return Nodes.Find(x => x.panelName == panelName) != null;
    }

    public bool TryMatchPanel(string parentName,string panelName,out BridgeObj bridgeObj, out UINodeBase uiNode)
    {
        var bridge = bridges.Find(x => x.inNode == parentName && x.outNode == panelName);
        if(bridge != null)
        {
            bridgeObj = poolDic[bridge].Allocate();//申请了个实例
        }
        else
        {
            bridgeObj = null;
        }
        uiNode = Nodes.Find(x => x.panelName == panelName);
        return uiNode != null;
    }

    protected virtual void Awake()
    {
        UIFacade.RegistGroup(this);
        RegistBridgePool();
    }
    /// <summary>
    /// bridge生成池
    /// </summary>
    protected virtual void RegistBridgePool()
    {
        foreach (var item in bridges)
        {
            poolDic[item] = new BridgePool(1, item);
        }
    }
    protected virtual void OnDestroy()
    {
        UIFacade.UnRegistGroup(this);
    }
}
