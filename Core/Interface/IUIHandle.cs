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
    internal interface IUIHandleInternal : IUIHandle
    {
        void Reset(UnityAction<UIHandle> onRelease);
        void RegistBridge(Bridge bridgeObj);
        void UnRegistBridge(Bridge obj);
    }
    public interface IUIHandle
    {
        IUIHandle Send(object data);
        IUIHandle RegistCallBack(UnityAction<IPanelBase, object> onCallBack);
        IUIHandle RegistCreate(UnityAction<IPanelBase> onCreate);
        IUIHandle RegistClose(UnityAction<IPanelBase> onClose);

    }
}