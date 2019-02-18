/*************************************************************************************   
    * 作    者：       zouhunter
    * 创建时间：       2018-04-26 08:38:24
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
public class UserControlPanel : ViewBaseComponent
{
    [SerializeField]
    private ButtonList m_List;

    [SerializeField]
    private ButtonItem m_buttonItem;

    public const byte keyword_infos = 1;

    public const byte keyword_onSelect = 2;


    protected override void OnInitialize()
    {
        //Binder.RegistValueChange<System.String[]>(m_List.SetList, keyword_infos);
        //Binder.RegistEvent(m_List.onSelectChanged, keyword_onSelect, m_List);
    }

    protected override void OnRecover()
    {
    }
}
