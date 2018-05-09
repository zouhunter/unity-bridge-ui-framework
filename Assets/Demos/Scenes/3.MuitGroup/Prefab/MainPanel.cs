/*************************************************************************************   
    * 作    者：       DefaultCompany
    * 时    间：       2018-04-20 07:20:59
    * 说    明：       1.本脚本由电脑自动生成
                       2.尽量使用MVVM模式
* ************************************************************************************/using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Binding;

///<summary>
///[代码说明信息]
///<summary>
public class MainPanel : BridgeUI.GroupPanel
{
	protected override void PropBindings ()
	{
		Binder.RegistEvent (m_openPanel01.onClick, "OpenPanel01");
		Binder.RegistEvent (m_openPanel02.onClick, "OpenPanel02", m_openPanel02);
		Binder.RegistEvent (m_openPanel03.onClick, "OpenPanel03", m_openPanel03);
		Binder.RegistMember<string> ("m_title.text", "title");
		Binder.RegistMember<string> ("m_info.text", "info");
		Binder.RegistEvent (m_switch.onValueChanged, "OnSwitch", m_switch);
		Binder.RegistEvent (m_slider.onValueChanged, "OnSliderChange", m_slider);
	}

    protected override void Awake ()
	{
		base.Awake ();
		m_close.onClick.AddListener (Close);
		m_switch.onValueChanged.AddListener (SetParent);
	}

	[SerializeField]
	private UnityEngine.UI.Button m_close;

	[SerializeField]
	private UnityEngine.UI.Button m_openPanel01;

	[SerializeField]
	private UnityEngine.UI.Button m_openPanel02;

	[SerializeField]
	private UnityEngine.UI.Button m_openPanel03;

	[SerializeField]
	private UnityEngine.UI.Text m_title;

	[SerializeField]
	private UnityEngine.UI.Text m_info;

	[SerializeField]
	private UnityEngine.UI.Toggle m_switch;

	[SerializeField]
	private UnityEngine.UI.Slider m_slider;

	protected void SetParent (System.Boolean arg0)
	{
	}
}
