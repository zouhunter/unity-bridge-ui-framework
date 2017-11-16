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
using System.Collections.Generic;
using System;

/// <summary>
/// 界面操作接口
/// </summary>
public sealed class UIFacade : IUIFacade
{
    public static UIFacade Instence
    {
        get
        {
            if (!instenceDic.ContainsKey(0) || instenceDic[0] == null)
            {
                instenceDic[0] = new UIFacade();
            }
            return instenceDic[0];
        }
    }
    public static UIFacade GetInstence(IPanelBase parentPanel)
    {
        if (parentPanel == null)
        {
            return Instence;
        }
        else
        {
            var id = parentPanel.InstenceID;
            if (!instenceDic.ContainsKey(id) || instenceDic[id] == null || instenceDic[id].parentPanel == null)
            {
                instenceDic[id] = new UIFacade(parentPanel);
            }
            return instenceDic[id];
        }
    }
    // Facade实例
    private static Dictionary<int, UIFacade> instenceDic = new Dictionary<int, UIFacade>();
    //生成器字典
    private static Dictionary<IPanelGroup, IPanelCreater> createrDic = new Dictionary<IPanelGroup, IPanelCreater>();
    // 已创建
    private static List<IPanelBase> createdPanels = new List<IPanelBase>();
    // 面板组
    private static List<IPanelGroup> groupList = new List<IPanelGroup>();
    //激活的handle
    private static List<UIHandle> activeBridges = new List<UIHandle>();
    //handle池
    private static UIHandlePool handlePool = new UIHandlePool();

    private IPanelBase parentPanel;

    private IPanelGroup currentGroup { get { return parentPanel == null ? null : parentPanel.Group; } }

    private UIFacade() { }

    private UIFacade(IPanelBase parentPanel) : this() { this.parentPanel = parentPanel; }

    public static void RegistGroup(IPanelGroup group)
    {
        if (!groupList.Contains(group))
        {
            groupList.Add(group);
            var creater = new PanelCreater(group.Trans);
            createrDic[group] = creater;
        }
    }

    public static void UnRegistGroup(IPanelGroup group)
    {
        if (groupList.Contains(group))
        {
            groupList.Remove(group);
            createrDic.Remove(group);
        }
    }

    public UIHandle OpenPanel(string panelName, object data = null)
    {
        string parentName = parentPanel == null ? "" : parentPanel.Name;
        var handle = handlePool.Allocate();

        foreach (var item in groupList)
        {
            var group = item;
            BridgeObj bridgeObj = null;
            UINodeBase uiNode = null;
            var match = item.TryMatchPanel(parentName, panelName, out bridgeObj, out uiNode);
            Debug.Log("TryMatchPanel:" + item);

            if (match)
            {
                handle.RegistBridge(bridgeObj);
                uiNode.OnCreate = (go) =>
                {
                    var b = go.GetComponent<IPanelBase>();
                    if (b != null)
                    {
                        b.HandleData(bridgeObj);
                    }
                };
                createrDic[group].CreatePanel(uiNode);
            }
        }

        return handle;
    }
}
