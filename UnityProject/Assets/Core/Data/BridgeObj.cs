using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
namespace BridgeUI.Model
{
    /// <summary>
    /// 记录面板之间的加载关联
    /// [同时用于之间的数据交流,使用时实例化对象]
    /// </summary>
    [System.Serializable]
    public class Bridge
    {

        #region 加载规则
        public string inNode;
        public ShowModel showModel;
        public string outNode;
        #endregion

        #region 实例使用

        public UnityAction<Queue<object>> onGet { get; set; }
        public Queue<object> dataQueue = new Queue<object>();
        public event UnityAction<Bridge> onRelease;
        public event UnityAction<IPanelBaseInternal, object> onCallBack;
        public event UnityAction<IPanelBaseInternal> onCreate;
        public IPanelBaseInternal InPanel { get; private set; }
        public IPanelBaseInternal OutPanel { get; private set; }

        public void Reset(IPanelBaseInternal parentPanel)
        {
            this.InPanel = parentPanel;
            this.onCreate = null;
            this.onGet = null;
            this.onCallBack = null;
            this.dataQueue.Clear();
        }

        public void Send(object data)
        {
            dataQueue.Enqueue(data);
            if (onGet != null)
            {
                onGet.Invoke(dataQueue);
            }
        }

        public void CallBack(IPanelBaseInternal panel, object data)
        {
            if (onCallBack != null) onCallBack.Invoke(panel, data);
        }

        public void Release()
        {
            if (onRelease != null)
            {
                onRelease(this);
            }
        }

        internal void OnCreatePanel(IPanelBaseInternal panel)
        {
            OutPanel = panel;
            if (onCreate != null)
            {
                onCreate.Invoke(panel);
            }
        }
        #endregion
    }
}