using System;
using BridgeUI.Binding;
using BridgeUI.Events;
///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-11-16 04:45:36
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/
///<summary>
///[代码说明信息]
///<summary>
public class VMIMGUIPanel_ViewModel1 : VMIMGUIPanel_ViewModel
{
    public VMIMGUIPanel_ViewModel1()
    {
        on_button_click = OnButtonClicked;
        label = "label from view model";
        title = "title from view model";
        Monitor<string>("label", OnLabelValueChanged);
    }

    private void OnLabelValueChanged(string arg0)
    {
        UnityEngine.Debug.Log("current label:" + arg0);
    }

    private void OnButtonClicked(IBindingContext panel, int arg0)
    {
        UnityEngine.Debug.Log( "[" + arg0 + "]");
    }
}
