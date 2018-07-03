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
using BridgeUI.Binding;

public class bindindMethodViewModel : ViewModel
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
