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
        UIBindingController BindingCtrl { get; }
        bool TryOpenOldPanel(string panelName, UIInfoBase uiInfo, IUIPanel parentPanel, out Bridge bridgeObj);
        bool CreateInfoAndBridge(string panelName, IUIPanel parentPanel, int index, UIInfoBase uiInfo, out Bridge bridgeObj);
        void CreatePanel(UIInfoBase uiNode, Bridge bridge, IUIPanel parentPanel);
        void CansaleInstencePanel(string panelName);
    }
}