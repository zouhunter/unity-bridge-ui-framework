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
    public interface IPanelVisitor
    {
        object Data { get; }
        IUIHandle uiHandle { get; }
        void Binding(IUIHandle uiHandle);
        void Recover();
    }

    public interface IUIHandle
    {
        string PanelName { get; }
        IUIPanel[] GetActivePanels();
        void Send(object data);
        void RegistCallBack(UnityAction<IUIPanel, object> onCallBack);
        void RemoveCallBack(UnityAction<IUIPanel, object> onCallBack);
        void RegistCreate(UnityAction<IUIPanel> onCreate);
        void RemoveCreate(UnityAction<IUIPanel> onCreate);
        void RegistClose(UnityAction<IUIPanel> onClose);
        void RemoveClose(UnityAction<IUIPanel> onClose);
        void Dispose();
    }

    internal interface IUIHandleInternal : IUIHandle
    {
        void Reset(string panelName,UnityAction<UIHandle> onRelease);
        void RegistBridge(Bridge bridgeObj);
        void UnRegistBridge(Bridge obj);
    }
   
}