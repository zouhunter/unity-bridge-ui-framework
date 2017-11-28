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
public class PanelGroup : MonoBehaviour, IPanelGroup
{
#if UNITY_EDITOR
    public List<GraphWorp> graphList;
#endif
    public LoadType loadType = LoadType.Prefab;
    public List<BundleUIInfo> b_nodes;
    public List<PrefabUIInfo> p_nodes;
    public List<Bridge> bridges;
    public Transform Trans { get { return transform; } }
    public List<UIInfoBase> Nodes { get { return activeNodes; } }

    private Bridge defultBridge;
    private Dictionary<Bridge, BridgePool> poolDic = new Dictionary<Bridge, BridgePool>();
    private List<IPanelBase> createdPanels = new List<IPanelBase>();
    private Dictionary<IPanelBase, Stack<IPanelBase>> hidedPanelStack = new Dictionary<IPanelBase, Stack<IPanelBase>>();
    private Dictionary<IPanelBase, Bridge> bridgeDic = new Dictionary<IPanelBase, Bridge>();
    private List<UIInfoBase> activeNodes;
    private IPanelCreater creater;

    void Awake()
    {
        creater = new PanelCreater();
        RegistUINodes();
        RegistBridgePool();
        TryAutoOpen("", Trans);
        UIFacade.RegistGroup(this);
    }

    private void OnEnable()
    {
        var created = createdPanels.ToArray();
        foreach (var item in created)
        {
            item.UnHide();
        }
    }

    private void OnDisable()
    {
        var created = createdPanels.ToArray();
        foreach (var item in created)
        {
            item.Hide();
        }
    }
    protected virtual void OnDestroy()
    {
        UIFacade.UnRegistGroup(this);
    }

    public Bridge InstencePanel(string parentName, string panelName, Transform root)
    {
        Bridge bridge = null;

        UIInfoBase uiNode = null;

        if (TryMatchPanel(parentName, panelName, out bridge, out uiNode))
        {
            uiNode.OnCreate = (go) =>
            {
                Utility.SetTranform(go.transform, uiNode.type.layer, root == null ? Trans : root);
                go.SetActive(true);
                var panel = go.GetComponent<IPanelBase>();
                if (panel != null)
                {
                    createdPanels.Add(panel);
                    TryRecordParentDic(parentName, panel);
                    bridgeDic.Add(panel, bridge);
                    InitPanel(panel, bridge, uiNode);
                    HandBridgeOptions(panel, bridge);
                }
            };
            creater.CreatePanel(uiNode);
        }
        return bridge;
    }

    public IPanelBase[] RetrivePanels(string panelName)
    {
        return createdPanels.FindAll(x => x.Name == panelName).ToArray();
    }

    #region private Functions
    private void HandBridgeOptions(IPanelBase panel, Bridge bridge)
    {
        TryHideParent(panel, bridge);
        TryHideMutexPanels(panel, bridge);
        TryAutoOpen(panel.Name, panel.Content);
    }
    /// <summary>
    /// 记录父子关系
    /// </summary>
    private void TryRecordParentDic(string parentPanel, IPanelBase childPanel)
    {
        if (string.IsNullOrEmpty(parentPanel)) return;
        var parent = createdPanels.Find(x => x.Name == parentPanel);
        if (parent != null)
        {
            parent.RecordChild(childPanel);
        }
    }

    /// <summary>
    /// 互斥面板自动隐藏
    /// </summary>
    /// <param name="childPanel"></param>
    /// <param name=""></param>
    /// <param name="bridge"></param>
    private void TryHideMutexPanels(IPanelBase childPanel, Bridge bridge)
    {
        if ((bridge.showModel & ShowModel.Mutex) == ShowModel.Mutex)
        {
            var mayBridges = bridges.FindAll(x => x.inNode == bridge.inNode);
            foreach (var bg in mayBridges)
            {
                var mayPanel = createdPanels.Find(x => x.Name == bg.outNode && x.UType.layer == childPanel.UType.layer && x != childPanel);
                if (mayPanel != null)
                {
                    if (mayPanel.IsShowing)
                    {
                        mayPanel.Hide();
                        if (!hidedPanelStack.ContainsKey(childPanel)){
                            hidedPanelStack[childPanel] = new Stack<IPanelBase>();
                        }
                        hidedPanelStack[childPanel].Push(mayPanel);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 自动打开子面板
    /// </summary>
    /// <param name="panel"></param>
    private void TryAutoOpen(string panelName, Transform content)
    {
        var autoBridges = bridges.FindAll(x => x.inNode == panelName && (x.showModel & ShowModel.Auto) == ShowModel.Auto);
        if (autoBridges != null)
        {
            foreach (var autoBridge in autoBridges)
            {
                InstencePanel(panelName, autoBridge.outNode, content);
            }
        }
    }
    /// <summary>
    /// 注册所有ui节点信息
    /// </summary>
    private void RegistUINodes()
    {
        activeNodes = new List<UIInfoBase>();

        if ((loadType & LoadType.Bundle) == LoadType.Bundle)
        {
            activeNodes.AddRange(b_nodes.ConvertAll<UIInfoBase>(x => x));
        }

        if ((loadType & LoadType.Prefab) == LoadType.Prefab)
        {
            activeNodes.AddRange(p_nodes.ConvertAll<UIInfoBase>(x => x));
        }
    }

    /// <summary>
    /// 按规则设置面板及父亲面板的状态
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="root"></param>
    /// <param name="bridge"></param>
    private void InitPanel(IPanelBase panel, Bridge bridge, UIInfoBase uiNode)
    {
        panel.UType = uiNode.type;
        panel.Group = this;
        panel.onDelete += OnDeletePanel;
        panel.HandleData(bridge);

    }

    private void TryHideParent(IPanelBase panel, Bridge bridge)
    {
        if ((bridge.showModel & ShowModel.HideBase) == ShowModel.HideBase)
        {
            var parent = createdPanels.Find(x => x.Name == bridge.inNode);
            if (parent != null)
            {
                panel.SetParent(Trans);
                if (parent.IsShowing)
                {
                    parent.Hide();
                    if (!hidedPanelStack.ContainsKey(panel))
                    {
                        hidedPanelStack[panel] = new Stack<IPanelBase>();
                    }
                    hidedPanelStack[panel].Push(parent);
                }
            }
        }
    }

    /// <summary>
    /// 找到信息源和bridge
    /// </summary>
    /// <param name="parentName"></param>
    /// <param name="panelName"></param>
    /// <param name="bridgeObj"></param>
    /// <param name="uiNode"></param>
    /// <returns></returns>
    private bool TryMatchPanel(string parentName, string panelName, out Bridge bridgeObj, out UIInfoBase uiNode)
    {
        uiNode = Nodes.Find(x => x.panelName == panelName);

        if (uiNode != null)// && uiNode.type.form == UIFormType.Fixed
        {
            if (uiNode.type.form == UIFormType.Fixed)
            {
                var oldPanel = createdPanels.Find(x => x.Name == panelName);
                if (oldPanel != null)
                {
                    bridgeObj = bridgeDic[oldPanel];
                    if (!oldPanel.IsShowing)
                    {
                        oldPanel.UnHide();
                        HandBridgeOptions(oldPanel, bridgeObj);
                    }
                    return false;
                }
            }
        }

        if (uiNode == null)
        {
            bridgeObj = null;
            return false;
        }

        bridgeObj = GetBridgeClamp(parentName, panelName);
        return uiNode != null && bridgeObj != null;
    }

    /// <summary>
    /// 获取可用的bridge
    /// </summary>
    /// <param name="parentName"></param>
    /// <param name="panelName"></param>
    /// <returns></returns>
    private Bridge GetBridgeClamp(string parentName, string panelName)
    {
        var mayBridge = bridges.FindAll(x => x.outNode == panelName);
        Bridge bridge = null;
        if (mayBridge != null && mayBridge.Count > 0)
        {
            var dirBridge = mayBridge.Find(x => x.inNode == parentName);
            if (dirBridge != null)
            {
                bridge = dirBridge;
            }
            else
            {
                bridge = mayBridge[0];
            }
            return poolDic[bridge].Allocate();
        }
        else
        {
            bridge = poolDic[defultBridge].Allocate();
            bridge.inNode = parentName;
            bridge.outNode = panelName;
            bridge.showModel = 0;
            return bridge;
        }

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
        defultBridge = new Bridge();
        poolDic[defultBridge] = new BridgePool(defultBridge);
    }

    /// <summary>
    /// 当删除一个面板时触发一些事
    /// </summary>
    /// <param name="panel"></param>
    private void OnDeletePanel(IPanelBase panel)
    {
        if (createdPanels.Contains(panel))
        {
            createdPanels.Remove(panel);
        }

        if (bridgeDic.ContainsKey(panel))
        {
            bridgeDic.Remove(panel);
        }

        if (panel.ChildPanels != null)
        {
            foreach (var item in panel.ChildPanels)
            {
                item.Close();
            }
        }

        if (hidedPanelStack.ContainsKey(panel))
        {
            var stack = hidedPanelStack[panel];
            if (stack != null)
            {
                while (stack.Count > 0)
                {
                    var item = stack.Pop();
                    if (item.IsAlive && !item.IsShowing)
                    {
                        item.UnHide();
                    }
                }
            }

        }
    }
    #endregion
}
