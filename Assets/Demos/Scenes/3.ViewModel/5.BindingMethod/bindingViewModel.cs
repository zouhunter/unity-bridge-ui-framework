using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI.Binding;

public partial class bindingViewModel
{
	public System.Collections.Generic.List<System.String> value {
		get {
			return GetValue<System.Collections.Generic.List<System.String>> ("value");
		}
		set {
			SetValue<System.Collections.Generic.List<System.String>> ("value", value);
		}
	}
}
public partial class bindingViewModel : ViewModel
{
}
