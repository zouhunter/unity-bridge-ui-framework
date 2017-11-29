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

public abstract class PanelBase : UIBehaviour, IPanelBase
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
    public GameObject Obj { get { return gameObject; } }
    public List<IPanelBase> ChildPanels
    {
        get
        {
            return childPanels;
        }
    }

    public bool IsShowing
    {
        get
        {
            return _isShowing && !IsDestroyed();
        }
    }

    public bool IsAlive
    {
        get
        {
            return _isAlive && !IsDestroyed();
        }
    }

    protected UIFacade selfFacade;

    protected Bridge bridge;
    protected List<IPanelBase> childPanels;
    public event UnityAction<IPanelBase> onDelete;
    protected bool _isShowing = true;
    private bool _isAlive = true;
    public void SetParent(Transform Trans)
    {
        Utility.SetTranform(transform, UType.layer, Trans);
    }
    public void CallBack(object data)
    {
        if (bridge != null)
        {
            bridge.CallBack(this,data);
        }
    }

    public void HandleData(Bridge bridge)
    {
        this.bridge = bridge;
        if (bridge != null)
        {
            HandleData(bridge.dataQueue);
            bridge.onGet = HandleData;
        }
    }

    protected virtual void HandleData(Queue<object> dataQueue)
    {
        if (dataQueue != null)
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
        base.Awake();
        selfFacade = UIFacade.CreatePanelFacade(this);
    }
    protected override void Start()
    {
        base.Start();
        bridge.OnCreatePanel(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _isAlive = false;
        _isShowing = false;

        if (bridge != null)
        {
            bridge.Release();
        }

        if (onDelete != null)
        {
            onDelete.Invoke(this);
        }
    }

    public virtual void Hide()
    {
        _isShowing = false;
        switch (UType.hideRule)
        {
            case HideRule.AlaphGameObject:
                AlaphGameObject(true);
                break;
            case HideRule.HideGameObject:
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
    public virtual void UnHide()
    {
        _isShowing = true;
        switch (UType.hideRule)
        {
            case HideRule.AlaphGameObject:
                AlaphGameObject(false);
                break;
            case HideRule.HideGameObject:
                gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public virtual void Close()
    {
        switch (UType.closeRule)
        {
            case CloseRule.DestroyImmediate:
                DestroyImmediate(gameObject);
                break;
            case CloseRule.DestroyDely:
                Destroy(gameObject, 0.02f);
                break;
            case CloseRule.DestroyNoraml:
                Destroy(gameObject);
                break;
            case CloseRule.HideGameObject:
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void RecordChild(IPanelBase childPanel)
    {
        if (childPanels == null)
        {
            childPanels = new List<IPanelBase>();
        }
        if (!childPanels.Contains(childPanel))
        {
            childPanel.onDelete += OnRemoveChild;
            childPanels.Add(childPanel);
        }
    }
    public void Cover()
    {
        var img = GetComponent<Image>();
        if (img == null)
        {
            img = gameObject.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.01f);
        }
        img.raycastTarget = true;
    }
    private void OnRemoveChild(IPanelBase childPanel)
    {
        if (childPanels != null && childPanels.Contains(childPanel))
        {
            childPanels.Remove(childPanel);
        }
    }

    private void AlaphGameObject(bool hide)
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        if(hide)
        {
            canvasGroup.alpha = UType.hideAlaph;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            Destroy(canvasGroup);
        }
       
    }

}
