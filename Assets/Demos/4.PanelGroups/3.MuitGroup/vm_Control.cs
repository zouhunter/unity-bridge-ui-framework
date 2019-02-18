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
using BridgeUI.Binding;
using System;

[System.Serializable]
public class vm_Control : ViewModel
{
    public System.Boolean show
    {
        get
        {
            return GetValue<System.Boolean>(keyword_show);
        }
        set
        {
            SetValue<System.Boolean>(keyword_show, value);
        }
    }

    public BridgeUI.Binding.PanelAction<System.Boolean> on_show_changed
    {
        get
        {
            return GetValue<BridgeUI.Binding.PanelAction<System.Boolean>>(keyword_on_show_changed);
        }
        set
        {
            SetValue<BridgeUI.Binding.PanelAction<System.Boolean>>(keyword_on_show_changed, value);
        }
    }

    public System.Single scale
    {
        get
        {
            return GetValue<System.Single>(keyword_scale);
        }
        set
        {
            SetValue<System.Single>(keyword_scale, value);
        }
    }

    public System.Single min_scale
    {
        get
        {
            return GetValue<System.Single>(keyword_min_scale);
        }
        set
        {
            SetValue<System.Single>(keyword_min_scale, value);
        }
    }

    public System.Single max_scale
    {
        get
        {
            return GetValue<System.Single>(keyword_max_scale);
        }
        set
        {
            SetValue<System.Single>(keyword_max_scale, value);
        }
    }

    public BridgeUI.Binding.PanelAction<System.Single> on_scale_changed
    {
        get
        {
            return GetValue<BridgeUI.Binding.PanelAction<System.Single>>(keyword_on_scale_changed);
        }
        set
        {
            SetValue<BridgeUI.Binding.PanelAction<System.Single>>(keyword_on_scale_changed, value);
        }
    }

    public System.String current_scale
    {
        get
        {
            return GetValue<System.String>(keyword_current_scale);
        }
        set
        {
            SetValue<System.String>(keyword_current_scale, value);
        }
    }

    public System.String show_title
    {
        get
        {
            return GetValue<System.String>(keyword_show_title);
        }
        set
        {
            SetValue<System.String>(keyword_show_title, value);
        }
    }

    public BridgeUI.Binding.PanelAction turn_green
    {
        get
        {
            return GetValue<BridgeUI.Binding.PanelAction>(keyword_turn_green);
        }
        set
        {
            SetValue<BridgeUI.Binding.PanelAction>(keyword_turn_green, value);
        }
    }

    public BridgeUI.Binding.PanelAction turn_red
    {
        get
        {
            return GetValue<BridgeUI.Binding.PanelAction>(keyword_turn_red);
        }
        set
        {
            SetValue<BridgeUI.Binding.PanelAction>(keyword_turn_red, value);
        }
    }
    protected byte keyword_show = 1;
    protected byte keyword_on_show_changed = 2;
    protected byte keyword_current_scale = 3;
    protected byte keyword_turn_red = 4;
    protected byte keyword_scale = 5;
    protected byte keyword_min_scale = 6;
    protected byte keyword_max_scale = 7;
    protected byte keyword_on_scale_changed = 9;
    protected byte keyword_show_title = 10;
    protected byte keyword_turn_green = 11;
    

    [SerializeField]
    protected GameObject m_prefab;

    [SerializeField]
    protected bool startActive;

    [SerializeField]
    protected float minScale;

    [SerializeField]
    protected float maxScale;

    [SerializeField]
    protected float currentScale;

    protected GameObject instence { get; set; }

    private bool initialized;

    public void Initialize()
    {
        if(!initialized)
        {
            initialized = true;
            show = startActive;
            scale = currentScale;
            min_scale = minScale;
            max_scale = maxScale;
            current_scale = "";
            UpdateShowText();
            on_scale_changed = SetGameObjectScale;
            on_show_changed = SetGameObjectVisiable;
            turn_green = SetCubeGreen;
            turn_red = SetCubeRed;
        }
       
    }

    protected void SetCubeGreen(BridgeUI.IUIPanel panel)
    {
        if (instence)
            instence.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    protected void SetCubeRed(BridgeUI.IUIPanel panel)
    {
        if (instence)
            instence.GetComponent<MeshRenderer>().material.color = Color.red;
    }
    public override void OnAfterBinding(BridgeUI.IUIPanel context)
    {
        Debug.Log("OnBinding:" + context);
        base.OnAfterBinding(context);
        Initialize();
        if (instence == null && m_prefab != null)
        {
            instence = GameObject. Instantiate(m_prefab);
            instence.SetActive(startActive);
            SetScaleInteranl(scale);
        }
    }

    public override void OnBeforeUnBinding(BridgeUI.IUIPanel context)
    {
        base.OnBeforeUnBinding(context);
        if (Contexts.Count == 0)
        {
            if (instence)
            {
                GameObject. Destroy(instence);
            }
        }
    }

    protected void SetGameObjectVisiable(BridgeUI.IUIPanel panel, bool isOn)
    {
        Debug.Log("SetGameObjectVisiable");
        show = isOn;
        UpdateShowText();
        if (instence)
            instence.gameObject.SetActive(show);
    }

    protected void UpdateShowText()
    {
        show_title = show ? "隐藏" : "显示";
    }

    protected void SetGameObjectScale(BridgeUI.IUIPanel panel, float value)
    {
        scale = value;
        SetScaleInteranl(scale);
    }

    protected void SetScaleInteranl(float scale)
    {
        current_scale = string.Format("当前cube 尺寸：" + scale.ToString("0.0"));
        if (instence)
            instence.gameObject.transform.localScale = Vector3.one * scale;
    }
}
