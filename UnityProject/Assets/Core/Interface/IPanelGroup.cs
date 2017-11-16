using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 面板组,用于记录面板及关联列表
/// </summary>
public interface IPanelGroup  {
    Transform Trans { get; }
    bool TryMatchPanel(string parentName, string panelName, out BridgeObj bridgeObj, out UINodeBase uiNode);
}
