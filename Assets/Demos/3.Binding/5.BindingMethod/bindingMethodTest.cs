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

public class bindingMethodTest : ViewBaseComponent
{
	[SerializeField]
	private bindingMethodTest m_bindingMethodTest;

	[SerializeField]
	private InputField input_field;

	[SerializeField]
	private UnityEngine.UI.InputField m_input_field;

	[SerializeField]
	private UnityEngine.UI.Button m_btn;

	[SerializeField]
	private BridgeUI.Control.ButtonListSelector m_list;

	[SerializeField]
	private UnityEngine.UI.Button m_close;

	protected byte keyword_value = 1;

	protected byte keyword_color = 2;

	protected byte keyword_ButtonClicked = 3;

	//protected override void PropBindings ()
	//{
	//	Binder.RegistValueChange<System.String[]> (m_bindingMethodTest.SetValue, keyword_value);
	//	Binder.RegistValueChange<UnityEngine.UI.ColorBlock> (x => m_close.colors = x, keyword_color);
	//	Binder.RegistValueChange<UnityEngine.UI.ColorBlock> (x => m_btn.colors = x, keyword_color);
	//	Binder.RegistEvent (m_btn.onClick, keyword_ButtonClicked);
	//	Binder.RegistValueChange<System.String[]> (x => m_list.options = x, keyword_value);
	//	//user edit
	//	Binder.RegistValueChange<System.String[]> (m_bindingMethodTest.SetValue, keyword_value);
	//	Binder.RegistValueChange<UnityEngine.UI.ColorBlock> (x => m_btn.colors = x, keyword_color);
	//	Binder.RegistEvent (m_btn.onClick, keyword_ButtonClicked);
	//	Binder.RegistValueChange<System.String[]> (x => m_list.options = x, keyword_value);
	//}

	public void SetValue (System.String[] value)
	{
		foreach (var item in value) {
			Debug.Log (item);
		}
	}

    protected override void OnInitialize()
    {
    }

    protected override void OnRecover()
    {
    }
}
