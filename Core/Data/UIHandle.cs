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

namespace BridgeUI
{
    public class UIHandle : IUIHandleInternal
    {
        private List<Bridge> bridges = new List<Bridge>();
        private UnityAction<IPanelBase, object> onCallBack { get; set; }
        private UnityAction<IPanelBase> onCreate { get; set; }
        private UnityAction<IPanelBase> onClose { get; set; }
        private UnityAction<UIHandle> onRelease { get; set; }
        public void Reset(UnityAction<UIHandle> onRelease)
        {
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

            if (onClose != null)
            {
                onClose(obj.OutPanel);
            }

            if (bridges.Count == 0)
            {
                Release();
            }
        }
        private void OnBridgeCallBack(IPanelBase panel, object data)
        {
            if (onCallBack != null)
            {
                onCallBack.Invoke(panel, data);
            }
        }

        private void OnCreatePanel(IPanelBase panel)
        {
            if (onCreate != null)
            {
                onCreate.Invoke(panel);
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
            this.onCallBack = null;
            this.onCreate = null;
            this.onClose = null;
        }

        public IUIHandle Send(object data)
        {
            foreach (var item in bridges)
            {
                item.Send(data);
            }
            return this;
        }

        public IUIHandle RegistCallBack(UnityAction<IPanelBase, object> onCallBack)
        {
            this.onCallBack = onCallBack;
            return this;
        }

        public IUIHandle RegistCreate(UnityAction<IPanelBase> onCreate)
        {
            this.onCreate = onCreate;
            return this;
        }

        public IUIHandle RegistClose(UnityAction<IPanelBase> onClose)
        {
            this.onClose = onClose;
            return this;
        }
    }
}