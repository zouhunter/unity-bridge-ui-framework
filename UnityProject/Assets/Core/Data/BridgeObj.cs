using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
/// <summary>
/// 记录面板之间的加载关联
/// [同时用于之间的数据交流,使用时实例化对象]
/// </summary>
[CreateAssetMenu(menuName = "UI/bridgeobj")]
public class BridgeObj : ScriptableObject {

    #region 加载规则
    public CloseRule closeRule;
    public HideRule hideRule;
    public OpenRule openRule;
    public string inNode;
    public string outNode;
    #endregion

    #region 实例使用
    private IPanelBase rootPanel;

    public UnityAction onRelease { get; set; }

    public Queue<object> dataQueue = new Queue<object>();

    public UnityAction<string,object> callBack;

    public UnityAction<Queue<object>> onGet;

    public void QueueSend(object data)
    {
        dataQueue.Enqueue(data);
        if(onGet != null)
        {
            onGet.Invoke(dataQueue);
        }
    }

    public void CallBack(object data)
    {
        if (callBack != null) callBack.Invoke(outNode, data);
    }

    public void Release()
    {
        if (onRelease != null) onRelease();
    }
    #endregion
}
