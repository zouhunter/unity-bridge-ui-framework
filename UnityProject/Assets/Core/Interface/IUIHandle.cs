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
public interface IUIHandleInternal
{
    void Reset(string panelName);
    void RegistBridge(Bridge bridgeObj);
    void UnRegistBridge(Bridge obj);
}
public interface IUIHandle
{
    UnityAction<IPanelBase, object> onCallBack { get; set; }
    UnityAction<IPanelBase> onCreate { get; set; }
    UnityAction<IPanelBase> onClose { get; set; }
    void Send(object data);
}
