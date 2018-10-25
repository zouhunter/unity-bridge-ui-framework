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
public class WorldUI : BridgeUI.SingleCloseAblePanel
{
    [SerializeField]
    private Button worldBtn;

    protected override void Awake()
    {
        base.Awake();
        worldBtn.onClick.AddListener(DebugOnClick);
    }

    private void DebugOnClick()
    {
        Debug.Log(this + ":" + worldBtn + ": onClicked");
    }
}
