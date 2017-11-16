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

public class UIHandle {
    private List<BridgeObj> bridges = new List<BridgeObj>();
    public UnityAction<string,object> callBack;

    public void RegistBridge(BridgeObj obj)
    {
        if(!bridges.Contains(obj))
        {
            obj.callBack += OnBridgeCallBack;
            bridges.Add(obj);
        }
    }
    public void UnRegistBridge(BridgeObj obj)
    {
        if(bridges.Contains(obj))
        {
            obj.callBack -= OnBridgeCallBack;
            bridges.Remove(obj);
        }
    }
    public void Send(object data)
    {
        foreach (var item in bridges)
        {
            item.QueueSend(data);
        }
    }

    private void OnBridgeCallBack(string panelName, object data)
    {
        if(callBack != null)
        {
            callBack.Invoke(panelName, data);
        }
    }
}
