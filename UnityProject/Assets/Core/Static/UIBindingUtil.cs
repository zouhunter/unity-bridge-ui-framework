using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BridgeUI
{
    public class UIBindingUtil
    {
        private static Dictionary<string, Dictionary<int, Func<PanelBase, object,IUIHandle>>> panelEventDic;
        public static void RegistPanelEvent(string parentPanel, int key, Func<PanelBase, object, IUIHandle> onOpen)
        {
            if (onOpen != null && !string.IsNullOrEmpty(parentPanel))
            {
                if (panelEventDic == null)
                {
                    panelEventDic = new Dictionary<string, Dictionary<int, Func<PanelBase, object,IUIHandle>>>();
                }
                if (!panelEventDic.ContainsKey(parentPanel))
                {
                    panelEventDic[parentPanel] = new Dictionary<int, Func<PanelBase, object,IUIHandle>>();
                }
                panelEventDic[parentPanel][key] = onOpen;
            }
        }
        public static void RemovePanelEvent(string parentPanel, int key, Func<PanelBase, object,IUIHandle> onOpen)
        {
            if (onOpen != null && panelEventDic != null && !string.IsNullOrEmpty(parentPanel))
            {
                if (panelEventDic.ContainsKey(parentPanel) && panelEventDic[parentPanel].ContainsKey(key) && panelEventDic[parentPanel][key] != null)
                {
                    panelEventDic[parentPanel].Remove(key);
                }
            }
        }
        public static IUIHandle InvokePanelEvent(PanelBase panel, int key, object data = null)
        {
            var parentName = panel.gameObject.name;////panel的名字不能改变
            if (panelEventDic.ContainsKey(parentName))
            {
                if (panelEventDic[parentName] != null && panelEventDic[parentName][key] != null)
                {
                  return  panelEventDic[parentName][key].Invoke(panel, data);
                }
            }
            return null;
        }
    }
}