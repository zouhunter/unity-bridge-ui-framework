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
/// <summary>
/// 用于写逻辑代码
/// </summary>
public class MainPanelViewModel : BridgeUI.Binding.ViewModelBase
{
    public readonly BindableProperty<string> title = new BindableProperty<string>();
}

public class MainPanel : GroupPanel
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
    
    protected override void Awake()
    {
        base.Awake();
        m_openPanel01.onClick.AddListener(() => this.Open(PanelNames.Panel01));
        m_openPanel02.onClick.AddListener(() => this.Open(PanelNames.Panel02));
        m_openPanel03.onClick.AddListener(() => this.Open(PanelNames.Panel03));
        m_close.onClick.AddListener(Close);

        Binder.Add<string>("title", (title)=> { m_title.text = title; });
    }
}
