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

///<summary>
///[代码说明信息]
///<summary>
public class MainPanel : BridgeUI.GroupPanel
{
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

	protected override void PropBindings ()
	{
		Binder.RegistButtonEvent (m_openPanel01, "OpenPanel01");
		Binder.RegistButtonEvent (m_openPanel02, "OpenPanel02");
		Binder.RegistButtonEvent (m_openPanel03, "OpenPanel03");
		Binder.RegistTextView (m_title, "title");
		Binder.RegistTextView (m_info, "info");
		Binder.RegistToggleEvent (m_switch, "OnSwitch");
		Binder.RegistSliderEvent (m_slider, "OnSliderChange");
	}

	protected void OnSwitch (bool isOn)
	{
	}

	protected void OnSliderChange (float value)
	{
	}

	[SerializeField]
	private UnityEngine.UI.Button m_close;

	protected override void InitComponents ()
	{
		m_close.onClick.AddListener (Close);
	}
}
