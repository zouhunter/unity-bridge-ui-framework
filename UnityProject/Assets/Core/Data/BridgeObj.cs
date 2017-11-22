using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
/// <summary>
/// 记录面板之间的加载关联
/// [同时用于之间的数据交流,使用时实例化对象]
/// </summary>
[System.Serializable]
public class Bridge  {

    #region 加载规则
    public string inNode;
    public ShowModel showModel;
    public string outNode;
    #endregion

    #region 实例使用
    public event UnityAction<Bridge> onRelease;

    public Queue<object> dataQueue = new Queue<object>();

    public UnityAction<string,object> callBack;

    public UnityAction<Queue<object>> onGet;

    public void Send(object data)
    {
        dataQueue.Enqueue(data);
        if(onGet != null){
            onGet.Invoke(dataQueue);
        }
    }

    public void CallBack(object data)
    {
        if (callBack != null) callBack.Invoke(outNode, data);
    }

    public void Release()
    {
        if (onRelease != null)
            onRelease(this);
    }
    #endregion
}
