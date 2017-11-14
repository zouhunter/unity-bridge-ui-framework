using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 记录面板之间的加载关联
/// [同时用于之间的数据交流,使用时实例化对象]
/// </summary>
public abstract class BridgeObj : ScriptableObject {

    #region 加载规则
    public CloseRule closeRule;
    public HideRule hideRule;
    public OpenRule openRule;
    public UINodeBase inNode;
    public UINodeBase outNode;
    #endregion

    #region 实例使用
    private static Dictionary<IPanelBase, BridgeObj> bridgeDic = new Dictionary<IPanelBase, BridgeObj>();
    public static BridgeObj GetBridge(IPanelBase rootPanel)
    {
        if(!bridgeDic.ContainsKey(rootPanel))
        {
            var instence = ScriptableObject.CreateInstance<BridgeObj>();
            bridgeDic.Add(rootPanel, instence);
        }
        return bridgeDic[rootPanel];
    }
    public static void DestroyBridge(IPanelBase rootPanel)
    {
        if(bridgeDic.ContainsKey(rootPanel))
        {
            bridgeDic.Remove(rootPanel);
        }
    }

    private IPanelBase rootPanel;
    public Queue<object> dataQueue = new Queue<object>();
    public void Send(object data)
    {

    }
    public void Back(object data)
    {

    }
    #endregion
}
