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

public class customEventViewModel : ViewModel
{
    #region 属性
    public BridgeUI.Binding.PanelAction<System.String,System.Int32> on_event_execute {
		get {
			return GetValue<BridgeUI.Binding.PanelAction<System.String,System.Int32>> ("on_event_execute");
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction<System.String,System.Int32>> ("on_event_execute", value);
		}
	}
    #endregion
    public customEventViewModel()
    {
        on_event_execute = OnEventTrigger;
    }
    private void OnEventTrigger(IBindingContext panel, string arg0, int arg1)
    {
        Debug.LogFormat(string.Format("str:{0}  int:{1}", arg0, arg1));
    }
}
