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

public class vm_Control : ViewModel
{
    #region 属性列表
    public System.Boolean show
    {
        get
        {
            return GetValue<System.Boolean>("show");
        }
        set
        {
            SetValue<System.Boolean>("show", value);
        }
    }
    public PanelAction<UnityEngine.UI.Toggle> on_show_changed
    {
        get
        {
            return GetValue<PanelAction<UnityEngine.UI.Toggle>>("on_show_changed");
        }
        set
        {
            SetValue<PanelAction<UnityEngine.UI.Toggle>>("on_show_changed", value);
        }
    }
    public System.Single scale
    {
        get
        {
            return GetValue<System.Single>("scale");
        }
        set
        {
            SetValue<System.Single>("scale", value);
        }
    }
    public System.Single min_scale
    {
        get
        {
            return GetValue<System.Single>("min_scale");
        }
        set
        {
            SetValue<System.Single>("min_scale", value);
        }
    }
    public System.Single max_scale
    {
        get
        {
            return GetValue<System.Single>("max_scale");
        }
        set
        {
            SetValue<System.Single>("max_scale", value);
        }
    }
    public PanelAction<UnityEngine.UI.Slider> on_scale_changed
    {
        get
        {
            return GetValue<PanelAction<UnityEngine.UI.Slider>>("on_scale_changed");
        }
        set
        {
            SetValue<PanelAction<UnityEngine.UI.Slider>>("on_scale_changed", value);
        }
    }
    public System.String show_title
    {
        get
        {
            return GetValue<System.String>("show_title");
        }
        set
        {
            SetValue<System.String>("show_title", value);
        }
    }
    public System.String current_scale
    {
        get
        {
            return GetValue<System.String>("current_scale");
        }
        set
        {
            SetValue<System.String>("current_scale", value);
        }
    }

    #endregion
    public GameObject m_prefab;
    private GameObject instence;

    protected override void InitPropertys()
    {
        base.InitPropertys();
        show = true;
        show_title = "隐藏";
        scale = 1;
        min_scale = 0.5f;
        max_scale = 2f;
        current_scale = "";
        on_scale_changed = SetGameObjectScale;
        on_show_changed = SetGameObjectVisiable;
    }

    public override void OnBinding(IBindingContext context)
    {
        base.OnBinding(context);
       
        if (instence == null && m_prefab != null)
        {
            instence = Instantiate(m_prefab);
        }
    }

    private void SetGameObjectVisiable(IBindingContext panel, Toggle sender)
    {
        show = sender.isOn;
        show_title = show ? "隐藏" : "显示";
        if (instence)
            instence.gameObject.SetActive(sender.isOn);
    }

    private void SetGameObjectScale(IBindingContext panel, Slider sender)
    {
        scale = sender.value;
        current_scale = string.Format("当前cube 尺寸：" + scale.ToString("0.0"));
        if (instence) instence.gameObject.transform.localScale = Vector3.one * sender.value;
    }

    private void OnDestroy()
    {
        if (instence)
        {
            Destroy(instence);
        }
    }
}
