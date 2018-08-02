using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI.Binding;

public class bindingViewChild : bindingViewModel
{
	#region 属性列表
	[DefultValue]
	public System.Collections.Generic.List<System.String> value {
		get {
			return GetValue<System.Collections.Generic.List<System.String>> ("value");
		}
		set {
			SetValue<System.Collections.Generic.List<System.String>> ("value", value);
		}
	}

	[DefultValue]
	public UnityEngine.UI.ColorBlock close_colors {
		get {
			return GetValue<UnityEngine.UI.ColorBlock> ("close_colors");
		}
		set {
			SetValue<UnityEngine.UI.ColorBlock> ("close_colors", value);
		}
	}

	[DefultValue]
	public BridgeUI.Binding.PanelAction ButtonClicked {
		get {
			return GetValue<BridgeUI.Binding.PanelAction> ("ButtonClicked");
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction> ("ButtonClicked", value);
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
		close_colors = colorBlock;
	}
}
