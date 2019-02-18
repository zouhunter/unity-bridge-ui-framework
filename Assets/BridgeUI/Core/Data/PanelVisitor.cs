using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeUI
{
    public class PanelVisitor : BridgeUI.IPanelVisitor
    {
        public object Data { get; private set; }

        public Action<IUIPanel, object> onCallBack { get; set; }

        public Action<IUIPanel> onCreate { get; set; }

        public Action<IUIPanel> onClose { get; set; }

        public IUIHandle uiHandle { get; private set; }

        public PanelVisitor()
        {

        }

        public PanelVisitor(object data)
        {
            this.Data = data;
        }

        public void Send(object data)
        {
            this.Data = data;
            if (this.uiHandle != null)
            {
                this.uiHandle.Send(data);
            }
        }

        private void OnCallBack(IUIPanel panel, object target)
        {
            if (this.onCallBack != null)
            {
                onCallBack(panel, target);
            }
        }

        private void OnClose(IUIPanel panel)
        {
            if (this.onClose != null){
                onClose(panel);
            }
        }

        private void OnCreate(IUIPanel panel)
        {
            if (this.onCreate != null)
            {
                onCreate(panel);
            }
        }

        public void Binding(IUIHandle uiHandle)
        {
            this.uiHandle = uiHandle;
            if(uiHandle != null)
            {
                uiHandle.RegistCallBack(OnCallBack);
                uiHandle.RegistClose(OnClose);
                uiHandle.RegistCreate(OnCreate);
            }
        }

        public void Recover()
        {
            if(uiHandle != null)
            {
                uiHandle.RemoveCallBack(OnCallBack);
                uiHandle.RemoveClose(OnClose);
                uiHandle.RemoveCreate(OnCreate);
                uiHandle = null;
            }
           
        }
    }
}
