/*************************************************************************************   
    * 作    者：       DefaultCompany
    * 时    间：       2018-04-06 11:15:08
    * 说    明：       1.本脚本由电脑自动生成
                       2.请尽量不要在其中写代码
                       3.更无法使用协程及高版本特性
* ************************************************************************************/

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
using BridgeUI;
using BridgeUI.Binding;
using System;
using System.Reflection;


public sealed class MainPanel : BridgeUI.GroupPanel
{

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Button m_close;

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Button m_openPanel01;

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Button m_openPanel02;

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Button m_openPanel03;

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Text m_title;

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Text m_info;

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Toggle m_switch;

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Slider m_slider;
    protected override void Awake()
    {
        base.Awake();
        //Binder.AddValue<object>("switcher", "m_switch.isOn");
        //Binder.AddValue<string>("info", "m_info.text");
        BindingContext = new MainPanelViewModel();
    }
    protected override void InitComponent()
    {
        base.InitComponent();
        this.m_close.onClick.AddListener(Close);
    }
    protected override void Binding()
    {
        base.Binding();
        Binder.AddText(m_title, "title");
        Binder.AddText(m_info, "info");
        Binder.AddButton(this.m_openPanel01, "OpenPanel01");
        Binder.AddButton(this.m_openPanel02, "OpenPanel02");
        Binder.AddButton(this.m_openPanel03, "OpenPanel03");
        Binder.AddToggle(this.m_switch, "Switch");
        Binder.AddSlider(this.m_slider, "progress");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Binder["m_switch.isOn"].Value = (this.m_switch.isOn == false);
        }
    }
}
