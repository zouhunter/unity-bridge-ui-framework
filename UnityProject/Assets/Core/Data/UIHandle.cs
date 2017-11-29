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
namespace BridgeUI
{
    public class UIHandle : IUIHandleInternal
    {
        private List<Bridge> bridges = new List<Bridge>();
        public UnityAction<IPanelBase, object> onCallBack { get; set; }
        public UnityAction<IPanelBase> onCreate { get; set; }
        public UnityAction<IPanelBase> onClose { get; set; }
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

        public void Send(object data)
        {
            foreach (var item in bridges)
            {
                item.Send(data);
            }
        }

        private void OnBridgeCallBack(IPanelBaseInternal panel, object data)
        {
            if (onCallBack != null)
            {
                onCallBack.Invoke(panel, data);
            }
        }

        private void OnCreatePanel(IPanelBaseInternal panel)
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
    }
}