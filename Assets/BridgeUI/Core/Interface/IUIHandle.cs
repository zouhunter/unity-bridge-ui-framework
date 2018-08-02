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
    public interface IUIHandle
    {
        string PanelName { get; }
        IUIPanel[] Contexts { get; }
        bool Active { get; }
        IUIHandle Send(object data);
        IUIHandle RegistCallBack(UnityAction<IUIPanel, object> onCallBack);
        IUIHandle RemoveCallBack(UnityAction<IUIPanel, object> onCallBack);
        IUIHandle RegistCreate(UnityAction<IUIPanel> onCreate);
        IUIHandle RegistClose(UnityAction<IUIPanel> onClose);

    }

    internal interface IUIHandleInternal : IUIHandle
    {
        void Reset(string panelName,UnityAction<UIHandle> onRelease);
        void RegistBridge(Bridge bridgeObj);
        void UnRegistBridge(Bridge obj);
    }
   
}