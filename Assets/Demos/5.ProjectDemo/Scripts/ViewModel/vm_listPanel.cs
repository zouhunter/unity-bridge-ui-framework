using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using BridgeUI.Binding;
using System;

public class vm_listPanel : ViewModel
{
	#region 属性列表
	[BridgeUI.Attributes.DefultValue]
	public System.String[] options {
		get {
			return GetValue<System.String[]> ("options");
		}
		set {
			SetValue<System.String[]> ("options", value);
		}
	}

	[BridgeUI.Attributes.DefultValue]
	public BridgeUI.Binding.PanelAction<System.Int32> on_selectid {
		get {
			return GetValue<BridgeUI.Binding.PanelAction<System.Int32>> ("on_selectid");
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction<System.Int32>> ("on_selectid", value);
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
			arg0.ToString (),
			options [arg0]
		});
	}
}
