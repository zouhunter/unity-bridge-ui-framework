///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-08-03 03:35:33
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/
using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

///<summary>
///[代码说明信息]
///<summary>
public class ListPanel : BridgeUI.SinglePanel
{
	protected const string keyword_options = "options";

	protected const string keyword_on_selectid = "on_selectid";

	[SerializeField]
	private BridgeUI.Control.ButtonListSelector m_List;

	[SerializeField]
	private UnityEngine.UI.Text m_Text;

	protected const string keyword_title = "title";

	protected override void PropBindings ()
	{
		Binder.RegistEvent (m_List.onSelectID, keyword_on_selectid);
		Binder.RegistValueChange<System.String[]> (x => m_List.options = x, keyword_options);
		Binder.RegistValueChange<System.String> (x=>m_Text.text=x, keyword_title);
	}
}
