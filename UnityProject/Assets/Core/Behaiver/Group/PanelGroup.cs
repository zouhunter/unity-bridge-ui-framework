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
public abstract class PanelGroup : MonoBehaviour, IPanelGroup
{
    public List<BridgeObj> bridges;
    private Dictionary<BridgeObj, BridgePool> poolDic = new Dictionary<BridgeObj, BridgePool>();
    private List<IPanelBase> createdPanels = new List<IPanelBase>();
    private Stack<IPanelBase> hidedPanels = new Stack<IPanelBase>();
    public abstract List<UINodeBase> Nodes { get; }
    public abstract List<PanelGroupObj> SubGroups { get; }
    public Transform Trans { get { return transform; } }

    private IPanelCreater creater;

    public BridgeObj InstencePanel(string parentName, string panelName, Transform root)
    {
        BridgeObj bridge = null;
        UINodeBase uiNode = null;

        if (TryMatchPanel(parentName, panelName, out bridge, out uiNode))
        {
            uiNode.OnCreate = (go) =>
            {
                Utility.SetTranform(go.transform, uiNode.type, root == null ? Trans : root);
                go.SetActive(true);
                var panel = go.GetComponent<IPanelBase>();
                if (panel != null)
                {
                    panel.UType = uiNode.type;
                    createdPanels.Add(panel);
                    InitPanelByBridge(panel, bridge);
                }
            };
            creater.CreatePanel(uiNode);
        }
        return bridge;

    }

    /// <summary>
    /// 按规则设置面板及父亲面板的状态
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="root"></param>
    /// <param name="bridge"></param>
    private void InitPanelByBridge(IPanelBase panel, BridgeObj bridge)
    {
        var parent = createdPanels.Find(x => x.Name == bridge.inNode);
        if ((bridge.showRule & ShowModel.HideParent) == ShowModel.HideParent)
        {
            if (parent != null)
            {
                Utility.SetTranform(panel.PanelTrans, panel.UType, Trans);
                parent.Hide();
                hidedPanels.Push(parent);
            }
        }
        panel.onDelete += OnDeletePanel;
        panel.HandleData(bridge);
    }
    private bool TryMatchPanel(string parentName, string panelName, out BridgeObj bridgeObj, out UINodeBase uiNode)
    {
        var bridge = bridges.Find(x => x.inNode == parentName && x.outNode == panelName);

        if (bridge != null)
        {
            bridgeObj = poolDic[bridge].Allocate();
        }
        else
        {
            bridgeObj = null;
        }

        uiNode = Nodes.Find(x => x.panelName == panelName);

        return uiNode != null && bridgeObj != null;
    }

    protected virtual void Awake()
    {
        creater = new PanelCreater();
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
            poolDic[item] = new BridgePool(item);
        }
    }
    protected virtual void OnDestroy()
    {
        UIFacade.UnRegistGroup(this);
    }

    public IPanelBase[] RetrivePanels(string panelName)
    {
        return createdPanels.FindAll(x => x.Name == panelName).ToArray();
    }

    private void OnDeletePanel(IPanelBase panel)
    {
        if (createdPanels.Contains(panel))
        {
            createdPanels.Remove(panel);
        }
        while (hidedPanels.Count > 0)
        {
            var item = hidedPanels.Pop();
            item.UnHide();
        }
    }
}
