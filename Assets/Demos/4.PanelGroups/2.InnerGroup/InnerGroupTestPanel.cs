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
using System;

///<summary>
///[代码说明信息]
///<summary>
public class InnerGroupTestPanel : ViewBaseComponent
{
    [SerializeField]
    private UnityEngine.UI.Button m_openPopUp;

    [SerializeField]
    private UnityEngine.UI.Button m_openPopUpSelf;

    protected void Awake()
    {
        m_openPopUp.onClick.AddListener(OpenPopUpTest);
        m_openPopUpSelf.onClick.AddListener(OpenPopupSelfOnly);
    }

    private void OpenPopupSelfOnly()
    {
        Open(PanelNames.PopupPanel, new string[] {
            "只加载同group的面板",
            "。。。预测加载不出来！"
        });
    }

    protected void OpenPopUpTest ()
	{
		UIFacade.Instence.Open (PanelNames.PopupPanel, new string[] {
			"子模块界面打开方案",
			"支持动态加载任意多个PanelGroup"
		});
	}


    protected override void OnInitialize()
    {
    }

    protected override void OnRecover()
    {
    }

   
}
