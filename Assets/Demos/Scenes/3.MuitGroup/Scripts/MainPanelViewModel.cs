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
    public readonly C_Button OpenPanel02Action;
    [Binding("OnSliderChange")]
    public readonly C_Slider sliderChange;

    [Binding("slider_value")]
    public readonly B_Float sliderValue;

    [Binding("OnSwitch")]
    public readonly C_Toggle toggleSwitch;


    private void OnSliderChange(IBindingContext panel, Slider sender)
    {
        sliderValue.Value = sender.value;
    }

    static T ChangeType<T>(object obj, T t)
    {
        return (T)obj;
    }

    public MainPanelViewModel()
    {
        toggleSwitch.RegistAction(OnToggleSwitch);
        sliderChange.RegistAction(OnSliderChange);
        OpenPanel01Action.RegistAction ((context) =>
        {
            var panel = context as PanelBase;
            panel.Open(PanelNames.Panel01);
        });
        OpenPanel02Action.Value = (context, sender) =>
        {
            var panel = context as PanelBase;
            panel.Open(PanelNames.Panel02);
        };
        this["OpenPanel03"] = new C_Button((context, sender) =>
         {
             var panel = context as PanelBase;
             panel.Open(PanelNames.Panel03);
         });
    }

    private void OnToggleSwitch(IBindingContext panel, Toggle sender)
    {
        switcherView.Value = sender.isOn;
        Debug.Log(sender.isOn);
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
    public readonly BindableProperty<string> title;
    public readonly BindableProperty<string> info;
    public readonly BindableProperty<bool> switcher;
    [Binding("OnSliderChange")]
    public readonly C_Slider sliderChange;
    public readonly C_PanelAction<Button> OpenPanel01;
    public readonly C_PanelAction<Button> OpenPanel02;
    //public readonly BindableProperty<PanelEvent> OpenPanel03 = new BindableProperty<PanelEvent>();
    public MainPanelViewModel_with_ID()
    {
        sliderChange.Value = OnSliderChanged;
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

    private void OnSliderChanged(IBindingContext panel, Slider sender)
    {
        Debug.Log(panel + "-->OnSliderChanged:" + sender.value);
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
