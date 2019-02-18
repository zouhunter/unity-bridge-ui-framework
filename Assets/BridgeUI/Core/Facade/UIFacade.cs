using UnityEngine;
using System.Collections.Generic;
using System;
using BridgeUI.Model;
using System.Reflection;
using UnityEngine.Events;

namespace BridgeUI
{
    /// <summary>
    /// 界面操作接口
    /// </summary>
    public class UIFacadeInternal : IUIFacade
    {
        //handle池
        private UIHandlePool handlePool = new UIHandlePool();
        private event UnityAction<IUIPanel> onCreate;
        private event UnityAction<IUIPanel> onClose;

        public void Open(string panelName, object data = null)
        {
            this.Open(null, panelName, data);
        }

        public void Open(IUIPanel parentPanel, string panelName, object data = null)
        {
            this.Open(parentPanel, panelName, -1, data);
        }
        public void Open(IUIPanel parentPanel, string panelName, int index, object data = null)
        {
            var handle = handlePool.Allocate(panelName);
            Open_Internal(handle, parentPanel, panelName, index, data);
        }


        public void Open(string panelName, IPanelVisitor uiData)
        {
            Open(null, panelName, uiData);
        }

        public void Open(IUIPanel parentPanel, string panelName, IPanelVisitor uiData)
        {
            var handle = handlePool.Allocate(panelName);
            object data = null;
            if (uiData != null)
            {
                data = uiData.Data;
                uiData.Binding(handle);
                handle.RegistOnRecover(uiData.Recover);
            }
            Open_Internal(handle, parentPanel, panelName, -1, data);
        }

        private void Open_Internal(UIHandle handle, IUIPanel parent, string panelName, int index, object data = null)
        {
            var currentGroup = parent == null ? null : parent.Group;
            var openOK = false;
            if (currentGroup != null)//限制性打开
            {
                openOK = InternalOpen(parent, currentGroup, handle, panelName, index);
            }
            else
            {
                var groupList = Utility.GetActivePanelGroups();
                foreach (var group in groupList)
                {
                    openOK |= InternalOpen(parent, group, handle, panelName, index);
                }
            }

            if (openOK)
            {
                if (data != null)
                {
                    handle.Send(data);
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("未打开成功，请检查配制信息");
#endif
            }
            handle.Dispose();
        }

        public void Hide(string panelName)
        {
            Hide(null, panelName);
        }

        public void Hide(IPanelGroup currentGroup, string panelName)
        {
            if (currentGroup != null)//限制性打开
            {
                InternalHide(currentGroup, panelName);
            }
            else
            {
                var groupList = Utility.GetActivePanelGroups();

                foreach (var group in groupList)
                {
                    InternalHide(group, panelName);
                }
            }
        }

        public void Close(IPanelGroup currentGroup, string panelName)
        {
            if (currentGroup != null)
            {
                InteralClose(currentGroup, panelName);
            }
            else
            {
                var groupList = Utility.GetActivePanelGroups();

                foreach (var group in groupList)
                {
                    InteralClose(group, panelName);
                }
            }
        }

        public void Close(string panelName)
        {
            Close(null, panelName);
        }

        public bool IsPanelOpen(string panelName)
        {
            bool globleHave = false;

            var groupList = Utility.GetActivePanelGroups();

            foreach (var item in groupList)
            {
                globleHave |= IsPanelOpen(item, panelName);
            }
            return globleHave;
        }

        public bool IsPanelOpen(IPanelGroup parentPanel, string panelName)
        {
            var panels = parentPanel.RetrivePanels(panelName);
            return (panels != null && panels.Count > 0);
        }

        public bool IsPanelOpen<T>(string panelName, out T[] panels)
        {
            var groupList = Utility.GetActivePanelGroups();
            var objpanels = new List<T>();
            var findPanel = false;
            foreach (var item in groupList)
            {
                T[] subpanels = null;
                if (IsPanelOpen(item, panelName, out subpanels))
                {
                    objpanels.AddRange(subpanels);
                    findPanel = true;
                }
            }

            if (findPanel)
            {
                panels = objpanels.ToArray();
            }
            else
            {
                panels = null;
            }

            return findPanel;
        }


        public bool IsPanelOpen<T>(IPanelGroup parentPanel, string panelName, out T[] panels)
        {
            var obj_panels = parentPanel.RetrivePanels(panelName);
            if (obj_panels != null && obj_panels.Count > 0)
            {
                panels = obj_panels.ToArray() as T[];
                return true;
            }
            else
            {
                panels = null;
                return false;
            }
        }


        public void RegistCreate(UnityAction<IUIPanel> onCreate)
        {
            if (onCreate == null) return;
            this.onCreate -= onCreate;
            this.onCreate += onCreate;
        }

        public void RegistClose(UnityAction<IUIPanel> onClose)
        {
            if (onClose == null) return;
            this.onClose -= onClose;
            this.onClose += onClose;
        }
        public void RemoveCreate(UnityAction<IUIPanel> onCreate)
        {
            if (onCreate == null) return;
            this.onCreate -= onCreate;
        }

        public void RemoveClose(UnityAction<IUIPanel> onClose)
        {
            if (onClose == null) return;
            this.onClose -= onClose;
        }

        internal bool InternalOpen(IUIPanel parentPanel, IPanelGroup group, IUIHandleInternal handle, string panelName, int index)
        {
            var Content = parentPanel == null ? null : parentPanel.Content;

            UIInfoBase uiInfo;

            group.Nodes.TryGetValue(panelName, out uiInfo);

            if (uiInfo == null)
            {
                return false;
            }

            Bridge bridge;


            if (group.TryOpenOldPanel(panelName, uiInfo, parentPanel,out bridge))
            {
                handle.RegistCreate(OnCreate);
                handle.RegistClose(OnClose);
                handle.RegistBridge(bridge);
                return true;
            }
            else
            {
                if (group.CreateInfoAndBridge(panelName, parentPanel, index, uiInfo, out bridge))
                {
                    handle.RegistCreate(OnCreate);
                    handle.RegistClose(OnClose);
                    handle.RegistBridge(bridge);

                    group.CreatePanel(uiInfo, bridge, parentPanel);
                    return true;
                }
            }

            return false;
        }
        private void InternalHide(IPanelGroup group, string panelName)
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
            group.CansaleInstencePanel(panelName);
        }

        private void OnCreate(IUIPanel panel)
        {
            if (this.onCreate != null)
            {
                this.onCreate.Invoke(panel);
            }
        }

        private void OnClose(IUIPanel panel)
        {
            if (this.onClose != null)
            {
                this.onClose.Invoke(panel);
            }
        }
    }

    /// <summary>
    /// 便于使用的单例
    /// </summary>
    public sealed class UIFacade
    {
        private static UIFacadeInternal _instence;
        public static UIFacadeInternal Instence
        {
            get
            {
                if (_instence == null)
                {
                    _instence = new UIFacadeInternal();
                }
                return _instence;
            }
        }
    }
}