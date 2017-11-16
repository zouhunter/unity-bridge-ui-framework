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
            if (!facadeDic.ContainsKey(0) || facadeDic[0] == null)
            {
                facadeDic[0] = new UIFacade();
            }
            return facadeDic[0];
        }
    }
    public static UIFacade CreatePanelFacade(IPanelBase parentPanel)
    {
        if (parentPanel == null)
        {
            return Instence;
        }
        else
        {
            var id = parentPanel.InstenceID;
            if (!facadeDic.ContainsKey(id) || facadeDic[id] == null || facadeDic[id].parentPanel == null)
            {
                facadeDic[id] = new UIFacade(parentPanel);
            }
            return facadeDic[id];
        }
    }
    public static void RemovePanelFacade(IPanelBase parentPanel)
    {
        if (parentPanel != null)
        {
            var id = parentPanel.InstenceID;
            facadeDic.Remove(id);
        }
    }
    // Facade实例
    private static Dictionary<int, UIFacade> facadeDic = new Dictionary<int, UIFacade>();
    // 面板组
    private static List<IPanelGroup> groupList = new List<IPanelGroup>();
    //激活的handle
    private static Dictionary<string,UIHandle> createdHandle = new Dictionary<string, UIHandle>();
    //handle池
    private static UIHandlePool handlePool = new UIHandlePool();

    private IPanelBase parentPanel;//如果有设置其为父transform

    private IPanelGroup currentGroup { get { return parentPanel == null ? null : parentPanel.Group; } }

    private UIFacade() { }

    private UIFacade(IPanelBase parentPanel) : this() { this.parentPanel = parentPanel; }

    public static void RegistGroup(IPanelGroup group)
    {
        if (!groupList.Contains(group))
        {
            groupList.Add(group);
        }
    }

    public static void UnRegistGroup(IPanelGroup group)
    {
        if (groupList.Contains(group))
        {
            groupList.Remove(group);
        }
    }

    public UIHandle Open(string panelName, object data = null)
    {
        if(createdHandle.ContainsKey(panelName))
        {
            return createdHandle[panelName];
        }
        else
        {
            string parentName = parentPanel == null ? "" : parentPanel.Name;
            var handle = handlePool.Allocate(panelName);
            handle.onRelease += AutoReleaseHandle;
            createdHandle.Add(panelName, handle);

            if (currentGroup != null)//限制性打开
            {
                InternalOpen(currentGroup, handle, parentName, panelName);
            }
            else
            {
                foreach (var group in groupList)
                {
                    InternalOpen(group, handle, parentName, panelName);
                }
            }

            return handle;
        }
     
    }

    private void InternalOpen(IPanelGroup group,UIHandle handle,string parentName,string panelName)
    {
        BridgeObj bridgeObj = group.InstencePanel(parentName, panelName, parentPanel);
        if (bridgeObj != null)
        {
            handle.RegistBridge(bridgeObj);
        }
    }

    private void AutoReleaseHandle(string panelName)
    {
        Debug.Log("Release Handle:" + panelName);
        if(createdHandle.ContainsKey(panelName))
        {
            createdHandle.Remove(panelName);
        }
    }

    public void Hide(string panelName)
    {
        if (currentGroup != null)//限制性打开
        {
            InternalHide(currentGroup, panelName);
        }
        else
        {
            foreach (var group in groupList)
            {
                InternalHide(group, panelName);
            }
        }
    }
    private void InternalHide(IPanelGroup group,string panelName)
    {
        var panels = group.RetrivePanels(panelName);
        if (panels != null)
        {
            foreach (var panel in panels)
            {
                panel.Hide();
            }
        }
    }

    public void Close(string panelName)
    {
        if (currentGroup != null)
        {
            InteralClose(currentGroup, panelName);
        }
        else
        {
            foreach (var group in groupList)
            {
                InteralClose(group, panelName);
            }
        }
    }

    private void InteralClose(IPanelGroup group, string panelName)
    {
        var panels = group.RetrivePanels(panelName);
        if (panels != null)
        {
            foreach (var panel in panels)
            {
                panel.Close();
            }
        }
    }
}
