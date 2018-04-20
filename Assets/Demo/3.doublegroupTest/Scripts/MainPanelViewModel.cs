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
    public readonly BindableProperty<string> title = new BindableProperty<string>();
    public readonly BindableProperty<string> info = new BindableProperty<string>();
    public readonly BindableProperty<bool> switcher = new BindableProperty<bool>();
    public readonly BindableProperty<ButtonEvent> OpenPanel01 = new BindableProperty<ButtonEvent>();
    public readonly BindableProperty<ButtonEvent> OpenPanel02 = new BindableProperty<ButtonEvent>();
    public readonly BindableProperty<ButtonEvent> OpenPanel03 = new BindableProperty<ButtonEvent>();

    public MainPanelViewModel()
    {
        OpenPanel01.Value = (panel, y,z)=>{
            panel.Open(PanelNames.Panel01);
        };
        OpenPanel02.Value = (panel, y, z) => {
            panel.Open(PanelNames.Panel02);
        };
        OpenPanel03.Value = (panel, y, z) => {
            panel.Open(PanelNames.Panel03);
        };
    }
}
