/*************************************************************************************   
    * 作    者：       zouhunter
    * 创建时间：       2018-07-02 02:20:02
    * 说    明：       1.部分代码自动生成
                       2.尽量使用MVVM模式
                       3.宏定义内会读成注释
* ************************************************************************************/using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

///<summary>
///[代码说明信息]
///<summary>
public class ControlPanel : BridgeUI.SinglePanel
{
	protected override void PropBindings ()
	{
		Binder.RegistMember<System.Boolean> ("m_show.isOn", "show");
		Binder.RegistEvent (m_show.onValueChanged, "on_show_changed", m_show);
		Binder.RegistMember<System.Single> ("m_scale.value", "scale");
		Binder.RegistEvent (m_scale.onValueChanged, "on_scale_changed", m_scale);
		Binder.RegistMember<System.String> ("m_show_title.text", "show_title");
		Binder.RegistMember<System.Single> ("m_scale.minValue", "min_scale");
		Binder.RegistMember<System.Single> ("m_scale.maxValue", "max_scale");
		Binder.RegistMember<System.String> ("m_current.text", "current_scale");
	}

	[SerializeField]
	private UnityEngine.UI.Toggle m_show;

	[SerializeField]
	private UnityEngine.UI.Slider m_scale;

	[SerializeField]
	private UnityEngine.UI.Text m_show_title;

	[SerializeField]
	private UnityEngine.UI.Text m_current;
}
