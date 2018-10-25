using UnityEngine;
using System.Collections.Generic;
using BridgeUI.Model;

namespace BridgeUI
{
    /// <summary>
    /// 面板组,用于记录面板及关联列表
    /// </summary>
    public interface IPanelGroup
    {
        Transform Trans { get; }
        Dictionary<string, UIInfoBase> Nodes { get; }
        List<IUIPanel> RetrivePanels(string panelName);
        UIBindingController bindingCtrl { get; }
        Bridge InstencePanel(IUIPanel parentPanel, string panelName, int index, Transform root);
        void CansaleInstencePanel(string panelName);
    }
}