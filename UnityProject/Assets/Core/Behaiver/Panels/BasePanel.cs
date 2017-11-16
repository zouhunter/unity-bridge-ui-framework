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
using System;
using System.Collections.Generic;

public class PanelBase :MonoBehaviour, IPanelBase
{
    public int InstenceID
    {
        get
        {
            return GetInstanceID();
        }
    }
    public string Name { get { return name; } }
    public IPanelGroup Group { get; set; }

    private BridgeObj bridge;

    public void CallBack(object data)
    {
        if(bridge != null)
        {
            bridge.CallBack(data);
        }
    }

    public void HandleData(BridgeObj bridge)
    {
        this.bridge = bridge;
        if (bridge)
        {
            HandleData(bridge.dataQueue);
            bridge.onGet = HandleData;
        }
    }

    private void HandleData(Queue<object> dataQueue)
    {
        if(dataQueue != null)
        {
            while (dataQueue.Count > 0)
            {
                var data = dataQueue.Dequeue();
                HandleData(data);
            }
        }
    }

    protected virtual void HandleData(object data)
    {
        Debug.Log(data);
    }

    private void OnDestroy()
    {
        if(bridge && bridge.onRelease != null)
        {
            bridge.onRelease();
        }
    }
}
