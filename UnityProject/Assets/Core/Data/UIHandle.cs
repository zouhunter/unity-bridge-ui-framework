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

public class UIHandle: IUIHandleInternal,IUIHandle
{
    public string panelName { get;private set; }
    private List<Bridge> bridges = new List<Bridge>();
    public UnityAction<IPanelBase, object> onCallBack { get; set; }
    public UnityAction<IPanelBase> onCreate { get; set; }
    public UnityAction<IPanelBase> onClose { get; set; }
    private UnityAction<string> onRelease { get; set; }
    public void Reset(string panelName)
    {
        this.onCallBack = null;
        this.onCreate = null;
        this.onClose = null;

        this.onRelease = onRelease;
        this.panelName = panelName;
    }

    public void RegistBridge(Bridge obj)
    {
        if(!bridges.Contains(obj))
        {
            obj.onCallBack += OnBridgeCallBack;
            obj.onRelease += UnRegistBridge;
            obj.onCreate += OnCreatePanel;
            bridges.Add(obj);
        }
    }

    public void UnRegistBridge(Bridge obj)
    {
        if(bridges.Contains(obj))
        {
            obj.onCallBack -= OnBridgeCallBack;
            obj.onRelease -= UnRegistBridge;
            obj.onCreate -= OnCreatePanel;
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

    private void OnBridgeCallBack(IPanelBase panel, object data)
    {
        if(onCallBack != null)
        {
            onCallBack.Invoke(panel, data);
        }
    }

    private void OnCreatePanel(IPanelBase panel)
    {
        if(onCreate != null)
        {
            onCreate.Invoke(panel);
        }
    }
    private void Release()
    {
        onCallBack = null;
        onClose = null;
        if (onRelease != null)
        {
            onRelease(panelName);
        }

    }
}
