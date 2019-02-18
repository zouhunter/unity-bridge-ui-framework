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
    private string[] optionStrs = new string[] {
        "Option A",
        "Option B",
        "Option C",
        "Option D",
    };

    private string[] infoStrs = new string[] {
        "直接继承ViewBaseComponent编厚View的脚本",
        "直接继承BindingViewBaseComponent，（分离控件绑定信息，逻辑",
        "使用BindingReference,ViewBase，（分离控件信息,逻辑）",
        "使用BindingReference,BindingViewBase，ViewModel （分离控件信息,绑定信息,逻辑）",
    };

    public override void OnAfterBinding(BridgeUI.IUIPanel context)
    {
        base.OnAfterBinding(context);
		options = optionStrs;
		on_selectid = OnSelectID;
	}

	private void OnSelectID (BridgeUI.IUIPanel panel, int arg0)
	{
		UIFacade.Instence.Open (panel as IUIPanel,"PopupPanel", new string[] {
            optionStrs[arg0],
            infoStrs [arg0]
		});
	}
}
