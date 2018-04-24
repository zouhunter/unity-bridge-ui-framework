/*************************************************************************************   
    * 作    者：       zouhunter
    * 创建时间：       2018-04-23 01:42:32
    * 说    明：       1.部分代码自动生成
                       2.尽量使用MVVM模式
* ************************************************************************************/using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//#if xLua
///<summary>
///[代码说明信息]
///<summary>
public class LuaPanel01 : BridgeUI.LuaPanel
{
	[SerializeField]
	private UnityEngine.UI.Button m_Button;

	[SerializeField]
	private UnityEngine.UI.ScrollRect m_ScrollView;

	[SerializeField]
	private UnityEngine.UI.Image m_Image;

	[SerializeField]
	private UnityEngine.RectTransform m_RawImage;

	[SerializeField]
	private UnityEngine.UI.Text m_Text;

	[SerializeField]
	private UnityEngine.UI.InputField m_InputField;

	[SerializeField]
	private UnityEngine.UI.Dropdown m_Dropdown;

	[SerializeField]
	private UnityEngine.UI.Toggle m_Toggle;

	[SerializeField]
	private UnityEngine.UI.Slider m_Slider;

	protected override void InitComponents ()
	{
	}

	protected override void PropBindings ()
	{
		Binder.RegistButtonEvent (m_Button, "on_button_clicked", "我是一个按扭");
		Binder.RegistSliderEvent (m_Slider, "on_slider_changed");
		Binder.RegistToggleEvent (m_Toggle, "on_toggle_switched");
		Binder.RegistTextView (m_Text, "text");
		Binder.RegistImageView (m_Image, "image");
	}

	protected override void Update ()
	{
		base.Update ();
		if (Input.GetMouseButtonDown (0)) {
			HandleData ("你好");
		}
	}
}
//#endif
