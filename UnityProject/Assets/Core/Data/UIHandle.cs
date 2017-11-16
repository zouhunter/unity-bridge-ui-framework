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
    public string panelName { get;private set; }

    private List<Bridge> bridges = new List<Bridge>();

    public UnityAction<string,object> callBack;

    public UnityAction<string> onRelease { get; set; }

    public void ResetHandle(string panelName)
    {
        this.panelName = panelName;
    }

    public void RegistBridge(Bridge obj)
    {
        if(!bridges.Contains(obj))
        {
            obj.callBack += OnBridgeCallBack;
            obj.onRelease += UnRegistBridge;
            bridges.Add(obj);
        }
    }

    public void UnRegistBridge(Bridge obj)
    {
        if(bridges.Contains(obj))
        {
            obj.callBack -= OnBridgeCallBack;
            obj.onRelease -= UnRegistBridge;
            bridges.Remove(obj);
        }

        if(bridges.Count == 0)
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

    private void OnBridgeCallBack(string panelName, object data)
    {
        if(callBack != null)
        {
            callBack.Invoke(panelName, data);
        }
    }

    public void Release()
    {
        if (onRelease != null)
        {
            onRelease(panelName);
        }
    }
}
