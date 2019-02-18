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

public class customEventTestPanel : ViewBaseComponent
{
	[SerializeField]
	private customComponent m_customCommponent;

    public const byte keyword_on_event_execute = 1;

    protected override void OnInitialize()
    {
    }

    protected override void OnRecover()
    {
    }

    //   protected override void PropBindings ()
    //{
    //	Binder.RegistEvent<System.String, System.Int32> (m_customCommponent.customEvent, keyword_on_event_execute);
    //	Binder.RegistEvent<System.String, System.Int32, customComponent> (m_customCommponent.customEvent, keyword_on_event_execute, m_customCommponent);
    //}

}
