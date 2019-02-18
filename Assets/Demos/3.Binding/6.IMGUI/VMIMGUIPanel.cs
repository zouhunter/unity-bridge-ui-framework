///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-08-29 11:35:55
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/
using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Events;

///<summary>
///[代码说明信息]
///<summary>
public class VMIMGUIPanel : ViewBaseComponent
{
    public const byte title_binding = 1;
    public const byte label_binding = 2;
    public const byte on_button_click_binding = 3;

    private string title;
	private string label;
	private IntEvent clickEvent = new IntEvent();
	private string lastText;
    
	//protected override void PropBindings ()
	//{
	//	base.PropBindings ();
	//	Binder.RegistValueChange<string> (x => title = x, title_binding);
	//	Binder.RegistValueChange<string> (x => label = x, label_binding);
	//	Binder.RegistEvent (clickEvent, on_button_click_binding);
 //   }

	private void OnGUI ()
	{
		GUILayout.Label (title);
		label = GUILayout.TextField (label);
		if (lastText != label) {
			lastText = label;
            //Binder.SetValue(label, label_binding);
        }
        if(GUILayout.Button("click!"))
        {
            clickEvent.Invoke(Random.Range(0,1000));
        }
	}

    protected override void OnInitialize()
    {
    }

    protected override void OnRecover()
    {
    }
}
