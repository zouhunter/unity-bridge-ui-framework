using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

/// <summary>
/// 面板组,用于记录面板及关联列表
/// </summary>
public interface IPanelGroup  {
    Transform Trans { get; }
    BridgeObj InstencePanel(string parentName, string panelName, Transform root);
    IPanelBase[] RetrivePanels(string panelName);
}
