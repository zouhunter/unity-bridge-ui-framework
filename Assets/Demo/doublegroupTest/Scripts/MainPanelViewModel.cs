#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-04-06 11:13:09
    * 说    明：       
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using BridgeUI;
using BridgeUI.Binding;
/// <summary>
/// 用于写逻辑代码
/// </summary>
public class MainPanelViewModel : BridgeUI.Binding.ViewModelBase
{
    public readonly BindableProperty<string> title = new BindableProperty<string>();
    public readonly BindableProperty<string> info = new BindableProperty<string>();
    public readonly BindableProperty<bool> switcher = new BindableProperty<bool>();

    public void OpenPanel01(object sender, RoutedEventArgs args)
    {
        var panel = args.OriginalSource as PanelBase;
        panel.Open(PanelNames.Panel01);
    }
    public void OpenPanel02(object sender, RoutedEventArgs args)
    {
        var panel = args.OriginalSource as PanelBase;
        panel.Open(PanelNames.Panel02);
    }
    public void OpenPanel03(object sender, RoutedEventArgs args)
    {
        var panel = args.OriginalSource as PanelBase;
        panel.Open(PanelNames.Panel03);
    }
}
