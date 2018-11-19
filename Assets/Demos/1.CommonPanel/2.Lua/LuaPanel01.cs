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

//
///<summary>
///[代码说明信息]
///<summary>
public class LuaPanel01 : BridgeUI.Extend.XLua.LuaPanel
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


    protected override void PropBindings ()
	{
		Binder.RegistValueChange<Sprite> (x => m_Image.sprite=x, "image");
		Binder.RegistValueChange<string> (x => m_Text.text=x, "text");
		Binder.RegistEvent (m_Button.onClick, "on_button_clicked", m_Button);
		Binder.RegistEvent(m_Toggle.onValueChanged, "on_toggle_switched", m_Toggle);
		Binder.RegistEvent (m_Slider.onValueChanged, "on_slider_switched", m_Slider);
		Binder.RegistValueChange<UnityEngine.Color> (x => m_btnPic.color=x, "btn_color");
		Binder.RegistEvent (m_InputField.onEndEdit, "on_inputfield_edited", m_InputField);
		Binder.RegistEvent (m_InputField.onEndEdit, "on_inputfield_edited1", m_InputField);
		Binder.RegistValueChange<UnityEngine.Color> (x=>m_btnPic.color=x, "btn_color1");
		Binder.RegistEvent (m_btnPic.onCullStateChanged, "on_cull_statechanged", m_InputField);
		Binder.RegistEvent (m_Dropdown.onValueChanged, "on_dropdown_switched", m_Dropdown);
        Binder.RegistEvent (m_ScrollView.onValueChanged, "on_scrollview_changed", m_InputField);
        Binder.RegistValueChange<UnityEngine.Color> (x => m_Image.color =x, "image_color");
	}

	protected override void Update ()
	{
		base.Update ();
		if (Input.GetMouseButtonDown (1)) {
			HandleData ("我是面板启动参数测试");
		}
	}
}
//#endif
