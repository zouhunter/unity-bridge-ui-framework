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

	protected override void PropBindings ()
	{
		Binder.RegistMember<List<System.String>> (m_bindingMethodTest.SetValue, "value");
	}

	public void SetValue (List<string> value)
	{
	}
}
