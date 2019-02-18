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
using BridgeUI.Model;
using System;
using System.Linq;

namespace BridgeUI
{
    public class UIHandle : IUIHandleInternal
    {
        private readonly List<IUIPanel> contexts = new List<IUIPanel>();
        private readonly List<Bridge> bridges = new List<Bridge>();
        private readonly List<UnityAction<IUIPanel, object>> onCallBack = new List<UnityAction<IUIPanel, object>>();
        private readonly List<UnityAction<IUIPanel>> onCreate = new List<UnityAction<IUIPanel>>();
        private readonly List<UnityAction<IUIPanel>> onClose = new List<UnityAction<IUIPanel>>();
        private UnityAction<UIHandle> onRelease { get; set; }
        private event UnityAction onRecover;

        public IUIPanel[] GetActivePanels()
        {
            return contexts.ToArray();
        }
        public string PanelName { get; private set; }

        public void Reset(string panelName, UnityAction<UIHandle> onRelease)
        {
            this.PanelName = panelName;
            this.onRelease = onRelease;
            this.onRecover = null;
        }

        public void RegistOnRecover(UnityAction onRecover)
        {
            this.onRecover += onRecover;
        }

        public void RegistBridge(Bridge obj)
        {
            if (!bridges.Contains(obj))
            {
                obj.onCallBack += OnBridgeCallBack;
                obj.onRelease += UnRegistBridge;
                obj.onCreate += OnCreatePanel;
                bridges.Add(obj);
            }
        }

        public void UnRegistBridge(Bridge obj)
        {
            if (bridges.Contains(obj))
            {
                obj.onCallBack -= OnBridgeCallBack;
                obj.onRelease -= UnRegistBridge;
                obj.onCreate -= OnCreatePanel;
                bridges.Remove(obj);
            }

            OnCloseCallBack(obj.OutPanel);

            if (bridges.Count == 0)
            {
                Dispose();
            }
        }

        private void OnBridgeCallBack(IUIPanel panel, object data)
        {
            var array = onCallBack.ToArray();

            foreach (var item in array)
            {
                item.Invoke(panel, data);
            }
        }

        private void OnCloseCallBack(IUIPanel panel)
        {
            contexts.Remove(panel);

            var array = onClose.ToArray();
            foreach (var item in array)
            {
                item.Invoke(panel);
            }
        }
        private void OnCreatePanel(IUIPanel panel)
        {
            contexts.Add(panel);

            var array = onCreate.ToArray();
            foreach (var item in array)
            {
                item.Invoke(panel);
            }
        }

        public void Dispose()
        {
            if (bridges.Count > 0)
            {
                for (int i = 0; i < bridges.Count; i++)
                {
                    var bridge = bridges[i];
                    bridge.onCallBack -= OnBridgeCallBack;
                    bridge.onRelease -= UnRegistBridge;
                    bridge.onCreate -= OnCreatePanel;
                }
                bridges.Clear();
            }

            onCallBack.Clear();
            onCreate.Clear();
            onClose.Clear();
            contexts.Clear();

            if (onRelease != null)
            {
                onRelease(this);
                onRelease = null;
            }

            if(onRecover != null)
            {
                onRecover.Invoke();
                this.onRecover = null;
            }
        }
        public void Send(object data)
        {
            foreach (var item in bridges)
            {
                item.Send(data);
            }
        }

        public void RegistCallBack(UnityAction<IUIPanel, object> onCallBack)
        {
            if (onCallBack == null) return;
            if (!this.onCallBack.Contains(onCallBack))
            {
                this.onCallBack.Add(onCallBack);
            }
        }

        public void RemoveCallBack(UnityAction<IUIPanel, object> onCallBack)
        {
            if (onCallBack == null) return;
            if (this.onCallBack.Contains(onCallBack))
            {
                this.onCallBack.Remove(onCallBack);
            }
        }

        public void RegistCreate(UnityAction<IUIPanel> onCreate)
        {
            if (onCreate != null && !this.onCreate.Contains(onCreate))
            {
                this.onCreate.Add(onCreate);
            }
        }

        public void RemoveCreate(UnityAction<IUIPanel> onCreate)
        {
            if (onCreate != null && this.onCreate.Contains(onCreate))
            {
                this.onCreate.Remove(onCreate);
            }
        }

        public void RegistClose(UnityAction<IUIPanel> onClose)
        {
            if (onClose != null && !this.onClose.Contains(onClose))
            {
                this.onClose.Add(onClose);
            }
        }

        public void RemoveClose(UnityAction<IUIPanel> onClose)
        {
            if (onClose != null && this.onClose.Contains(onClose))
            {
                this.onClose.Remove(onClose);
            }
        }

    }
}