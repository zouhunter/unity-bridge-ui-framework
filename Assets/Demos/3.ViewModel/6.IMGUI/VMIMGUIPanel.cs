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
public class VMIMGUIPanel : BridgeUI.SinglePanel
{
    public const string title_binding = "title";
    public const string label_binding = "label";
    public const string on_button_click_binding = "on_button_click";

    private string title;
	private string label;
	private IntEvent clickEvent = new IntEvent();
	private string lastText;
    
	protected override void PropBindings ()
	{
		base.PropBindings ();
		Binder.RegistValueChange<string> (x => title = x, title_binding);
		Binder.RegistValueChange<string> (x => label = x, label_binding);
		Binder.RegistEvent (clickEvent, on_button_click_binding);
    }

	private void OnGUI ()
	{
		GUILayout.Label (title);
		label = GUILayout.TextField (label);
		if (lastText != label) {
			lastText = label;
            Binder.SetValue(label, label_binding);
        }
        if(GUILayout.Button("click!"))
        {
            clickEvent.Invoke(Random.Range(0,1000));
        }
	}
}
