using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using BridgeUI.Binding;
using System;

public class ListPanel_ViewModel0 : ListPanel_ViewModel
{
	[SerializeField]
	private string[] optionStrs;

	public override void OnBinding (IBindingContext context)
	{
		base.OnBinding (context);
		options = optionStrs;
		on_selectid = OnSelectID;
	}

	private void OnSelectID (IBindingContext panel, int arg0)
	{
		UIFacade.Instence.Open (panel as IUIPanel,"PopupPanel", new string[] {
			"小提示-" + arg0.ToString (),
			options [arg0]
		});
	}
}
