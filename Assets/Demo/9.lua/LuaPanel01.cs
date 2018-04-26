/*************************************************************************************   
    * 作    者：       zouhunter
    * 创建时间：       2018-04-23 01:42:32
    * 说    明：       1.部分代码自动生成
                       2.尽量使用MVVM模式
* ************************************************************************************/using BridgeUI;
using BridgeUI.Binding;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

//#if xLua
///<summary>
///[代码说明信息]
///<summary>
public class LuaPanel01 : BridgeUI.LuaPanel
{
	[SerializeField]
	private UnityEngine.UI.Button m_Button;

	[SerializeField]
	private UnityEngine.UI.Slider m_Slider;

	[SerializeField]
	private UnityEngine.UI.Toggle m_Toggle;

	[SerializeField]
	private UnityEngine.UI.Dropdown m_Dropdown;

	[SerializeField]
	private UnityEngine.UI.InputField m_InputField;

	[SerializeField]
	private UnityEngine.UI.Text m_Text;

	[SerializeField]
	private UnityEngine.RectTransform m_RawImage;

	[SerializeField]
	private UnityEngine.UI.Image m_Image;

	[SerializeField]
	private UnityEngine.UI.ScrollRect m_ScrollView;

	[SerializeField]
	private UnityEngine.UI.Image m_btnPic;

	protected override void InitComponents ()
	{
		m_Button.onClick.AddListener (on_button_clicked);
		m_Slider.onValueChanged.AddListener (on_slider_switched);
	}

	private void on_button_clicked ()
	{
	}

	protected override void PropBindings ()
	{
		Binder.RegistMember<Sprite> ("m_Image.sprite", "image");
		Binder.RegistMember<string> ("m_Text.text", "text");
		Binder.RegistEvent (m_Button.onClick, "on_button_clicked", "我是一个按扭");
		Binder.RegistEvent (m_Toggle.onValueChanged, "on_toggle_switched", "我是一个Toggle");
		Binder.RegistEvent (m_Slider.onValueChanged, "on_slider_switched");
		Binder.RegistMember<UnityEngine.Color> ("m_btnPic.color", "btn_color");
	}

	protected override void Update ()
	{
		base.Update ();
		if (Input.GetMouseButtonDown (1)) {
			HandleData ("我是面板启动参数测试");
		}
	}

	protected void on_slider_switched (Single arg0)
	{
	}
}
//#endif
