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
        List<UIInfoBase> Nodes { get; }
        List<IPanelBaseInternal> GetPanelsByName(string panelName);
        Bridge InstencePanel(IPanelBaseInternal parentPanel, string panelName, Transform root);
        IPanelBaseInternal[] RetrivePanels(string panelName);
    }
}