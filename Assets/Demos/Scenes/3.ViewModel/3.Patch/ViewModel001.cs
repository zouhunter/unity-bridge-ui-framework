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

public class ViewModel001 : ViewModel
{
    #region 属性列表
    public PanelAction<UnityEngine.UI.Button> onClick
    {
        get
        {
            return GetValue<PanelAction<UnityEngine.UI.Button>>("onClick");
        }
        set
        {
            SetValue<PanelAction<UnityEngine.UI.Button>>("onClick", value);
        }
    }
    public System.String title
    {
        get
        {
            return GetValue<System.String>("title");
        }
        set
        {
            SetValue<System.String>("title", value);
        }
    } 
    public System.Int32 fontSize
    {
        get
        {
            return GetValue<System.Int32>("fontSize");
        }
        set
        {
            SetValue<System.Int32>("fontSize", value);
        }
    }
    public System.String info
    {
        get
        {
            return GetValue<System.String>("info");
        }
        set
        {
            SetValue<System.String>("info", value);
        }
    }
    #endregion

   [SerializeField] private string _title;
   [SerializeField] private string _info;
   [SerializeField] private int _fontSize;

    private void OnEnable()
    {
        title = _title;
        info = _info;
        fontSize = _fontSize;
        onClick = OpenPanel01;
    }

    private void OpenPanel01(IBindingContext panel, Button sender)
    {
        Debug.Log("OpenPanel01");
        title = "panel:" + panel;
        info = "sender:" + sender;
    }
}
