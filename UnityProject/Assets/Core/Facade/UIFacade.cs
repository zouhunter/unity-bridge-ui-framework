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
    public static UIFacade GetInstence(PanelBase parentPanel)
    {
        var id = parentPanel.GetInstanceID();
        if(!instenceDic.ContainsKey(id) || instenceDic[id] == null || instenceDic[id].parentPanel == null){
            instenceDic[id] = new UIFacade(parentPanel);
        }
        return instenceDic[id];
    }
    // Facade实例
    private static Dictionary<int, UIFacade> instenceDic = new Dictionary<int, UIFacade>();
    // 面板组
    private static List<PanelGroup> groupList = new List<PanelGroup>();

    private PanelBase parentPanel;
    
    private UIFacade() { }
    private UIFacade(PanelBase parentPanel) { this.parentPanel = parentPanel; }

    public static void RegistGroup(PanelGroup group)
    {
        if(!groupList.Contains(group))
        {
            groupList.Add(group);
        }
    }

    public static void UnRegistGroup(PanelGroup group)
    {
        if (groupList.Contains(group))
        {
            groupList.Remove(group);
        }
    }
}
