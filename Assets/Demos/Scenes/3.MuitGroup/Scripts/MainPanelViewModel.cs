#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-04-06 11:13:09
    * 说    明：       
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using BridgeUI;
using BridgeUI.Binding;

/// <summary>
/// 用于写逻辑代码
/// </summary>
public class MainPanelViewModel : BridgeUI.Binding.ViewModelBase
{
    [Binding("title")]
    public readonly B_String titleStr;
    [Binding("info")]
    public readonly B_String infoStr;
    [Binding("switcher")]
    public readonly B_Bool switcherView;
    [Binding("OpenPanel01")]
    public readonly C_PanelAction OpenPanel01Action;
    [Binding("OpenPanel02")]
    public readonly C_PanelAction<Button> OpenPanel02Action;

    static T ChangeType<T>(object obj, T t)
    {
        return (T)obj;
    }

    public MainPanelViewModel()
    {
        OpenPanel01Action.Value = (context) =>
        {
            var panel = context as PanelBase;
            panel.Open(PanelNames.Panel01);
        };
        OpenPanel02Action.Value = (context, sender) =>
        {
            var panel = context as PanelBase;
            panel.Open(PanelNames.Panel02);
        };
        //OpenPanel03.Value = (panel, z) => {
        //    panel.Open(PanelNames.Panel03);
        //};
        this["OpenPanel03"] = new BindableProperty<CallBack<Button>>((context, sender) =>
         {
             var panel = context as PanelBase;
             panel.Open(PanelNames.Panel03);
         });
    }
    public override void OnBinding(IBindingContext context)
    {
        base.OnBinding(context);
        Debug.Log("绑定到：" + context);
    }
    public override void OnUnBinding(IBindingContext context)
    {
        base.OnUnBinding(context);
        Debug.Log("解除绑于：" + context);
    }
}


/// <summary>
/// 用于写逻辑代码
/// </summary>
public class MainPanelViewModel_with_ID : BridgeUI.Binding.ViewModelBase
{
    public readonly BindableProperty<string> title = new BindableProperty<string>();
    public readonly BindableProperty<string> info = new BindableProperty<string>();
    public readonly BindableProperty<bool> switcher = new BindableProperty<bool>();
    public readonly C_PanelAction<Button> OpenPanel01 = new C_PanelAction<Button>();
    public readonly C_PanelAction<Button> OpenPanel02 = new C_PanelAction<Button>();
    //public readonly BindableProperty<PanelEvent> OpenPanel03 = new BindableProperty<PanelEvent>();
    public MainPanelViewModel_with_ID()
    {
        OpenPanel01.Value = (context, sender) =>
        {
            var panel = context as PanelBase;

            if (!panel.IsOpen(0))
            {
                panel.Open(0);
            }
            else
            {
                panel.Close(0);
            }
        };
        OpenPanel02.Value = (context, sender) =>
        {
            var panel = context as PanelBase;

            panel.Open(1);
        };
        //OpenPanel03.Value = (panel, z) => {
        //    panel.Open(PanelNames.Panel03);
        //};
        this["OpenPanel03"] = new C_PanelAction<Button>((context, sender) =>
        {
            var panel = context as PanelBase;

            panel.Open(2);
        });
    }
    public override void OnBinding(IBindingContext context)
    {
        base.OnBinding(context);
        Debug.Log("绑定到：" + context);
    }
    public override void OnUnBinding(IBindingContext context)
    {
        base.OnUnBinding(context);
        Debug.Log("解除绑于：" + context);
    }
}
