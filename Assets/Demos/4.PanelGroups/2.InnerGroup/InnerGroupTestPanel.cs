/*************************************************************************************   
    * 作    者：       zouhunter
    * 创建时间：       2018-05-09 10:16:02
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
public class InnerGroupTestPanel : BridgeUI.SinglePanel
{
	protected override void Awake ()
	{
		base.Awake ();
		m_openPopUp.onClick.AddListener (OpenPopUpTest);
	}

	protected void OpenPopUpTest ()
	{
		UIFacade.Instence.Open (PanelNames.PopupPanel, new string[] {
			"子模块界面打开方案",
			"支持动态加载任意多个PanelGroup"
		});
	}

	[SerializeField]
	private UnityEngine.UI.Button m_openPopUp;
}
