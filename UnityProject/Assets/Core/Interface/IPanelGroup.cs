using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 面板组,用于记录面板及关联列表
/// </summary>
public interface IPanelGroup  {
    Transform Trans { get; }
    List<UIInfoBase> Nodes { get; }
    Bridge InstencePanel(string parentName, string panelName, Transform root);
    IPanelBase[] RetrivePanels(string panelName);
}
