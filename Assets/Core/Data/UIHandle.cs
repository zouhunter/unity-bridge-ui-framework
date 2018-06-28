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
        private readonly List<Bridge> bridges = new List<Bridge>();
        private readonly List<UnityAction<IPanelBase, object>> onCallBack = new List<UnityAction<IPanelBase, object>>();
        private readonly List<UnityAction<IPanelBase>> onCreate = new List<UnityAction<IPanelBase>>();
        private readonly List<UnityAction<IPanelBase>> onClose = new List<UnityAction<IPanelBase>>();
        private UnityAction<UIHandle> onRelease { get; set; }

        public IPanelBase[] Contexts
        {
            get
            {
                return bridges.Select(x => x.OutPanel).ToArray();
            }
        }
        public bool Active
        {
            get
            {
                return bridges.Count > 0;
            }
        }
        public string PanelName { get; private set; }

        public void Reset(string panelName, UnityAction<UIHandle> onRelease)
        {
            this.PanelName = panelName;
            this.onRelease = onRelease;
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
                Release();
            }
        }


        private void OnBridgeCallBack(IPanelBase panel, object data)
        {
            var array = onCallBack.ToArray();
            foreach (var item in array)
            {
                item.Invoke(panel, data);
            }
        }
        private void OnCloseCallBack(IPanelBase panel)
        {
            var array = onClose.ToArray();
            foreach (var item in array)
            {
                item.Invoke(panel);
            }
        }
        private void OnCreatePanel(IPanelBase panel)
        {
            var array = onCreate.ToArray();
            foreach (var item in array)
            {
                item.Invoke(panel);
            }
        }

        private void Release()
        {
            if (onRelease != null)
            {
                onRelease(this);
            }
            Clean();
        }

        private void Clean()
        {
            this.onCallBack.Clear();
            this.onCreate.Clear();
            this.onClose.Clear();
        }

        public IUIHandle Send(object data)
        {
            foreach (var item in bridges){
                item.Send(data);
            }
            return this;
        }

        public IUIHandle RegistCallBack(UnityAction<IPanelBase, object> onCallBack)
        {
            if (onCallBack == null) return null;
            if (!this.onCallBack.Contains(onCallBack))
            {
                this.onCallBack.Add(onCallBack);
            }
            return this;
        }

        public IUIHandle RegistCreate(UnityAction<IPanelBase> onCreate)
        {
            if (onCreate == null) return null;
            if (!this.onCreate.Contains(onCreate))
            {
                this.onCreate.Add(onCreate);
            }
            return this;
        }

        public IUIHandle RegistClose(UnityAction<IPanelBase> onClose)
        {
            if (onClose == null) return null;
            if (!this.onClose.Contains(onClose))
            {
                this.onClose.Add(onClose);
            }
            return this;
        }
    }
}