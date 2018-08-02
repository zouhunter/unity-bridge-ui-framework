using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI.Binding;

public class bindingViewChild : bindingViewModel
{
	#region 属性列表
	[DefultValue]
	public BridgeUI.Binding.PanelAction ButtonClicked {
		get {
			return GetValue<BridgeUI.Binding.PanelAction> ("ButtonClicked");
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction> ("ButtonClicked", value);
		}
	}

	[DefultValue]
	public System.String[] value {
		get {
			return GetValue<System.String[]> ("value");
		}
		set {
			SetValue<System.String[]> ("value", value);
		}
	}

	[DefultValue]
	public UnityEngine.UI.ColorBlock color {
		get {
			return GetValue<UnityEngine.UI.ColorBlock> ("color");
		}
		set {
			SetValue<UnityEngine.UI.ColorBlock> ("color", value);
		}
	}

	#endregion 属性列表
	public bindingViewChild ()
	{
		var colorBlock = new UnityEngine.UI.ColorBlock ();
		colorBlock.disabledColor = Color.gray;
		colorBlock.normalColor = Color.white;
		colorBlock.highlightedColor = Color.red;
		colorBlock.pressedColor = Color.yellow;
		colorBlock.colorMultiplier = 1;
		color = colorBlock;
		value = new string[] {
			"a",
			"b",
			"c"
		};
	}
}
