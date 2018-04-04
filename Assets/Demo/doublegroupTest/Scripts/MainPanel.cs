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

public class BindAttribute : Attribute
{
    public string key;
    public bool get;
    public bool set;
    public BindAttribute(string key, bool get = true, bool set = true)
    {
        this.key = key;
        this.get = get;
        this.set = set;
    }
}

/// <summary>
/// 用于写逻辑代码
/// </summary>
public class MainPanelViewModel : BridgeUI.Binding.ViewModelBase
{
    public readonly BindableProperty<string> title = new BindableProperty<string>();
    public readonly BindableProperty<bool> switcher = new BindableProperty<bool>();

    public void OpenPanel01(object sender, RoutedEventArgs args)
    {
        var panel = args.OriginalSource as PanelBase;
        panel.Open(PanelNames.Panel01);
    }
}

public class MainPanel : GroupPanel, IPropertyChanged
{
    [SerializeField]
    private Button m_close;
    [SerializeField]
    private Button m_openPanel01;
    [SerializeField]
    private Button m_openPanel02;
    [SerializeField]
    private Button m_openPanel03;
    [SerializeField]
    private Text m_title;
    [SerializeField]
    private Text m_info;
    [SerializeField]
    private Toggle m_switch;
 
    protected override void Awake()
    {
        base.Awake();
        m_close.onClick.AddListener(Close);

        m_openPanel02.onClick.AddListener(() => this.Open(PanelNames.Panel02));
        m_openPanel03.onClick.AddListener(() => this.Open(PanelNames.Panel03));

        Binder.AddValue<bool>("switcher","m_switch.isOn");
        Binder.AddValue<string>("title", "m_title.text");
        Binder.AddValue<string>("info", "m_info.text");
        Binder.AddButton(m_openPanel01, "OpenPanel01");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            Binder["m_switch.isOn"].Value = !m_switch.isOn;
        }
        //Binder.Set<string>("info", UnityEngine.Random.Range(0, 100).ToString());

    }
}
