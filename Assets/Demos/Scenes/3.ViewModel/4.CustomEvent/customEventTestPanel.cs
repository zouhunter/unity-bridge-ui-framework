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

public class customEventTestPanel : BridgeUI.PanelBase
{
	[SerializeField]
	private customComponent m_customCommponent;

	protected override void PropBindings ()
	{
		Binder.RegistEvent<System.String, System.Int32> (m_customCommponent.customEvent, "on_event_execute");
		Binder.RegistEvent<System.String, System.Int32, customComponent> (m_customCommponent.customEvent, "on_event_execute", m_customCommponent);
	}
	
}
