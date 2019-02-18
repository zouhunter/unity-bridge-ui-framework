///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-07-05 12:18:47
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/

using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using BridgeUI.Control;

///<summary>
///[代码说明信息]
///<summary>
public class WorldUI : ViewBaseComponent
{
    [SerializeField]
    private Button worldBtn;
    [SerializeField]
    private Button closeBtn;


    private void DebugOnClick()
    {
        Debug.Log(this + ":" + worldBtn + ": onClicked");
    }

    protected override void OnInitialize()
    {
        worldBtn.onClick.AddListener(DebugOnClick);
        closeBtn.onClick.AddListener(Close);
    }

    protected override void OnRecover()
    {
        worldBtn.onClick.RemoveListener(DebugOnClick);
        closeBtn.onClick.RemoveListener(Close);
    }
}
