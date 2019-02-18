using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI.Binding;
using System;

public class bindingViewChild : bindingMethodTest_ViewModel
{
	#region 属性列表
    public string inputfield
    {
        get
        {
            return GetValue<string>(keyword_inputfield);
        }
        set
        {
            SetValue<string>(keyword_inputfield, value);
        }
    }
    #endregion 属性列表

    public const byte keyword_inputfield = 1;

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
        inputfield = "输入框内容";
        ButtonClicked = OnButtonClicked;
        GetBindableProperty<string>(keyword_inputfield).RegistValueChanged(OnInputFieldChanged);
    }

    private void OnInputFieldChanged(string arg0)
    {
        Debug.Log(arg0);
    }

    private void OnButtonClicked(BridgeUI.IUIPanel panel)
    {
        Debug.Log(panel);
    }
}
