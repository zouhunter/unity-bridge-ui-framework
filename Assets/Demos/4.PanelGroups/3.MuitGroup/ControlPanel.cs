///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-07-03 02:24:32
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/
using BridgeUI;
using BridgeUI.Binding;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

///<summary>
///[代码说明信息]
///<summary>
public class ControlPanel : BindingViewBaseComponent
{
    [SerializeField]
	private UnityEngine.UI.Toggle m_show;

	[SerializeField]
	private UnityEngine.UI.Text m_current;

	[SerializeField]
	private UnityEngine.UI.Button m_red;

	[SerializeField]
	private UnityEngine.UI.Slider m_scale;

	[SerializeField]
	private UnityEngine.UI.Text m_show_title;


    [SerializeField]
    private vm_Control m_viewModel;

    public const byte keyword_show = 1;
    public const byte keyword_on_show_changed = 2;
    public const byte keyword_current_scale = 3;
    public const byte keyword_turn_red = 4;
    public const byte keyword_scale = 5;
    public const byte keyword_min_scale = 6;
    public const byte keyword_max_scale = 7;
    public const byte keyword_on_scale_changed = 9;
    public const byte keyword_show_title = 10;


    protected override void OnInitialize()
    {
        Binder.RegistValueChange<System.Boolean>(x => m_show.isOn = x, keyword_show);
        Binder.RegistEvent<System.Boolean>(m_show.onValueChanged, keyword_on_show_changed);
        Binder.RegistValueChange<System.String>(x => m_current.text = x, keyword_current_scale);
        Binder.RegistEvent(m_red.onClick, keyword_turn_red);
        Binder.RegistValueChange<System.Single>(x => m_scale.value = x, keyword_scale);
        Binder.RegistValueChange<System.Single>(x => m_scale.minValue = x, keyword_min_scale);
        Binder.RegistValueChange<System.Single>(x => m_scale.maxValue = x, keyword_max_scale);
        Binder.RegistEvent<System.Single>(m_scale.onValueChanged, keyword_on_scale_changed);
        Binder.RegistValueChange<System.String>(x => m_show_title.text = x, keyword_show_title);

        VM = m_viewModel;
    }
}
