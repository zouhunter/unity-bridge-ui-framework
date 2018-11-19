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
	#region 属性列表
	protected System.String[] options {
		get {
			return GetValue<System.String[]> (keyword_options);
		}
		set {
			SetValue<System.String[]> (keyword_options, value);
		}
	}

	protected BridgeUI.Binding.PanelAction<System.Int32> on_selectid {
		get {
			return GetValue<BridgeUI.Binding.PanelAction<System.Int32>> (keyword_on_selectid);
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction<System.Int32>> (keyword_on_selectid, value);
		}
	}

	#endregion 属性列表
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
		UIFacade.Instence.Open ("PopupPanel", new string[] {
			"小提示-" + arg0.ToString (),
			options [arg0]
		});
	}

	protected const string keyword_options = "options";

	protected const string keyword_on_selectid = "on_selectid";
}
