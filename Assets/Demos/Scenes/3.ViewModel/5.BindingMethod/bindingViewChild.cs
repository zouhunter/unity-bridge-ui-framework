using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bindingViewChild : bindingViewModel
{
	public UnityEngine.UI.ColorBlock close_colors {
		get {
			return GetValue<UnityEngine.UI.ColorBlock> ("close_colors");
		}
		set {
			SetValue<UnityEngine.UI.ColorBlock> ("close_colors", value);
		}
	}

	public BridgeUI.Binding.PanelAction ButtonClicked {
		get {
			return GetValue<BridgeUI.Binding.PanelAction> ("ButtonClicked");
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction> ("ButtonClicked", value);
		}
	}
}
