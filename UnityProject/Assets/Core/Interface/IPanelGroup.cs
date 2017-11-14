using UnityEngine;
using System.Collections;

/// <summary>
/// 面板组,用于记录面板及关联列表
/// </summary>
public interface IPanelGroup  {
    Transform Trans { get; }
    bool Contains(string panelName);
    BridgeObj OpenPanel(IPanelBase parentPanel,string panelName, object data);
}
