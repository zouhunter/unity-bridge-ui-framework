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

public abstract class PanelBase :UIBehaviour, IPanelBase
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
    public abstract Transform Content { get; }
    public UIType UType { get; set; }
    public Transform PanelTrans
    {
        get
        {
            return transform;
        }
    }
    
    protected UIFacade selfFacade;

    protected Bridge bridge;

    public event UnityAction<IPanelBase> onDelete;

    public void SetParent(Transform Trans)
    {
        Utility.SetTranform(PanelTrans, UType.layer, Trans);
    }
    public void CallBack(object data)
    {
        if(bridge != null)
        {
            bridge.CallBack(data);
        }
    }

    public void HandleData(Bridge bridge)
    {
        this.bridge = bridge;
        if (bridge != null){
            HandleData(bridge.dataQueue);
            bridge.onGet = HandleData;
        }
    }

    protected virtual void HandleData(Queue<object> dataQueue)
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
    }

    protected override void Awake()
    {
        selfFacade = UIFacade.CreatePanelFacade(this);
    }

    protected override void OnDestroy()
    {
        if(bridge != null){
            bridge.Release();
        }
        if(onDelete != null){
            onDelete.Invoke(this);
        }
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
    public virtual void UnHide()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        Destroy(gameObject);
    }

}
