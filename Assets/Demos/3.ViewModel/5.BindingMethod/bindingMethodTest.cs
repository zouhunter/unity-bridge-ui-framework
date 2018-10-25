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

public class bindingMethodTest : BridgeUI.SingleCloseAblePanel
{
	[SerializeField]
	private bindingMethodTest m_bindingMethodTest;

	[SerializeField]
	private UnityEngine.UI.Button m_btn;

	[SerializeField]
	private BridgeUI.Control.ButtonListSelector m_list;

	protected override void PropBindings ()
	{
		Binder.RegistMember<System.String[]> (m_bindingMethodTest.SetValue, "value");
		Binder.RegistMember<UnityEngine.UI.ColorBlock> (x => m_close.colors = x, "color");
		Binder.RegistMember<UnityEngine.UI.ColorBlock> (x => m_btn.colors = x, "color");
		Binder.RegistEvent (m_btn.onClick, "ButtonClicked");
		Binder.RegistMember<System.String[]> (x => m_list.options = x, "value");
	}

	public void SetValue (System.String[] value)
	{
		foreach (var item in value) {
			Debug.Log (item);
		}
	}
}
