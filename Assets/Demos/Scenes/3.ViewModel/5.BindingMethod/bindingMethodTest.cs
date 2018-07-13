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

public class bindingMethodTest : BridgeUI.SinglePanel
{
	[SerializeField]
	private bindingMethodTest m_bindingMethodTest;

	[SerializeField]
	private UnityEngine.UI.Button m_close;

	[SerializeField]
	private UnityEngine.UI.Button m_btn;

	protected override void PropBindings ()
	{
		Binder.RegistMember<List<System.String>> (m_bindingMethodTest.SetValue, "value");
		Binder.RegistMember<UnityEngine.UI.ColorBlock> (x => m_close.colors = x, "close_colors");
		Binder.RegistMember<UnityEngine.UI.ColorBlock> (x => m_btn.colors = x, "close_colors");
		Binder.RegistEvent (m_btn.onClick, "ButtonClicked");
	}

	public void SetValue (List<string> value)
	{
	}
}
