/*************************************************************************************   
    * 作    者：       zouhunter
    * 创建时间：       2018-05-04 11:17:35
    * 说    明：       1.部分代码自动生成
                       2.尽量使用MVVM模式
                       3.宏定义内会读成注释
* ************************************************************************************/using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

///<summary>
///[代码说明信息]
///<summary>
public class GenericTypePanel : BridgeUI.SinglePanel
{
	[SerializeField]
	private SubComponent m_subcomponent;

	protected override void InitComponents ()
	{
	}

	protected override void PropBindings ()
	{
		Binder.RegistMember<System.String[]> ("m_subcomponent.stringTest", "array");
		Binder.RegistMember<List<System.String>> ("m_subcomponent.listTest", "list");
		Binder.RegistMember<Dictionary<System.String,System.String>> ("m_subcomponent.dicTest", "dic");
	}
}
