/*************************************************************************************   
    * 作    者：       zouhunter
    * 创建时间：       2018-06-29 04:37:51
    * 说    明：       1.部分代码自动生成
                       2.尽量使用MVVM模式
                       3.宏定义内会读成注释
* ************************************************************************************/using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Binding;
using System;

///<summary>
///[代码说明信息]
///<summary>
public class VMUsePanel : ViewBaseComponent
{
	[SerializeField]
	private UnityEngine.UI.Button m_click;

	[SerializeField]
	private UnityEngine.UI.Text m_title;

	[SerializeField]
	private UnityEngine.UI.Text m_info;

	protected const byte keyword_onClick = 1;

	protected const byte keyword_title =2;

	protected const byte keyword_fontSize = 3;

	protected const byte keyword_info = 4;

    protected override void OnInitialize()
    {
    }

    protected override void OnRecover()
    {
    }

    //protected override void PropBindings ()
    //{
    //	Binder.RegistEvent (m_click.onClick, keyword_onClick, m_click);
    //	Binder.RegistValueChange<System.String> (x=>m_title.text=x, keyword_title);
    //	Binder.RegistValueChange<System.Int32> (x=>m_title.fontSize=x, keyword_fontSize);
    //	Binder.RegistValueChange<System.String> (x=>m_info.text=x, keyword_info);
    //}
}
