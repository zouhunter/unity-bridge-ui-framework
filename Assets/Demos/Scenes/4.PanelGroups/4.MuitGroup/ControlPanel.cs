///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-07-03 02:24:32
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/
using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

///<summary>
///[代码说明信息]
///<summary>
public class ControlPanel : BridgeUI.SinglePanel
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

	protected override void PropBindings ()
	{
		Binder.RegistMember<System.Boolean> (x => m_show.isOn = x, "show");
		Binder.RegistEvent<System.Boolean> (m_show.onValueChanged, "on_show_changed");
		Binder.RegistMember<System.String> (x => m_current.text = x, "current_scale");
		Binder.RegistEvent (m_red.onClick, "turn_red");
		Binder.RegistMember<System.Single> (x => m_scale.value = x, "scale");
		Binder.RegistMember<System.Single> (x => m_scale.minValue = x, "min_scale");
		Binder.RegistMember<System.Single> (x => m_scale.maxValue = x, "max_scale");
		Binder.RegistEvent<System.Single> (m_scale.onValueChanged, "on_scale_changed");
		Binder.RegistMember<System.String> (x => m_show_title.text = x, "show_title");
	}
}
