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
public class ListPanel : BridgeUI.Binding.BindingViewBaseComponent
{
    protected const byte keyword_title = 0;

    protected const byte keyword_options = 1;

    protected const byte keyword_on_selectid = 2;

    [SerializeField]
    private BridgeUI.Control.ButtonListSelector m_List;

    [SerializeField]
    private UnityEngine.UI.Text m_Text;

    protected override void OnInitialize()
    {
        m_List.Initialize();
        Binder.RegistEvent(m_List.onSelectID, keyword_on_selectid);
        Binder.RegistValueChange<System.String[]>(x => m_List.options = x, keyword_options);
        Binder.RegistValueChange<System.String>(x => m_Text.text = x, keyword_title);
        VM = new ListPanel_ViewModel0();
    }

    protected override void OnRecover()
    {
        m_List.Recover();
    }
}
