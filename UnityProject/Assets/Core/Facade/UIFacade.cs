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

/// <summary>
/// 界面操作接口
/// </summary>
public sealed class UIFacade
{
    public static UIFacade Instence
    {
        get
        {
            if(!instenceDic.ContainsKey(0) || instenceDic[0] == null)
            {
                instenceDic[0] = new UIFacade();
            }
            return instenceDic[0];
        }
    }

    public static UIFacade GetInstence(IPanelBase parentPanel)
    {
        if(parentPanel == null)
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

    public static UIFacade GetInstence(string panelGroupObjPath)
    {
        if(string.IsNullOrEmpty(panelGroupObjPath))
        {
            return Instence;
        }
        else
        {
            var group = Resources.Load<PanelGroupObj>(panelGroupObjPath);
            RegistGroup(group);
            var id = group.GetInstanceID();
            if (!instenceDic.ContainsKey(id) || instenceDic[id] == null || instenceDic[id].parentPanel == null){
                instenceDic[id] = new UIFacade();
            }
            return instenceDic[id];
        }
    }
    // Facade实例
    private static Dictionary<int, UIFacade> instenceDic = new Dictionary<int, UIFacade>();
    // 面板组
    private static List<IPanelGroup> groupList = new List<IPanelGroup>();

    private IPanelBase parentPanel;
    
    private UIFacade() { }
    private UIFacade(IPanelBase parentPanel):this() { this.parentPanel = parentPanel; }

    public static void RegistGroup(IPanelGroup group)
    {
        if(!groupList.Contains(group))
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
}
