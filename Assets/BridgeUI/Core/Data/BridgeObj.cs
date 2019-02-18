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
    public struct BridgeInfo
    {
        #region 加载规则
        public string inNode;
        public string outNode;
        public short index;
        public ShowMode showModel;
        //public ScriptableObject viewModel;
        #endregion
        public BridgeInfo(string inNode,string outNode,ShowMode showModel,/* ScriptableObject viewModel, */short index)
        {
            this.inNode = inNode;
            this.outNode = outNode;
            this.showModel = showModel;
            this.index = index;
            //this.viewModel = viewModel;
        }
    }

    public class Bridge
    {
        #region 实例使用
        public BridgeInfo Info { get; set; }
        public UnityAction<Queue<object>> onGet { get; set; }
        public Queue<object> dataQueue = new Queue<object>();
        public event UnityAction<Bridge> onRelease;
        public event UnityAction<IUIPanel, object> onCallBack;
        public event UnityAction<IUIPanel> onCreate;
        public IUIPanel InPanel { get; private set; }
        public IUIPanel OutPanel { get; private set; }
        private UnityAction<Bridge> onReleaseFromPool { get; set; }
        public Bridge(UnityAction<Bridge> onReleaseFromPool)
        {
            this.onReleaseFromPool = onReleaseFromPool;
        }

        public void ResetInfo(BridgeInfo info)
        {
            this.Info = new BridgeInfo(info.inNode, info.outNode, info.showModel, info.index);
            this.onCreate = null;
            this.onGet = null;
            this.onCallBack = null;
            this.dataQueue.Clear();

        }

        public void SetInPanel(IUIPanel parentPanel)
        {
            this.InPanel = parentPanel;
            if (InPanel != null)
            {
                Info = new BridgeInfo(parentPanel.Name, Info.outNode, Info.showModel, Info.index);
            }
            else
            {
                Info = new BridgeInfo("", Info.outNode, Info.showModel, Info.index);
            }
        }

        public void Send(object data)
        {
            dataQueue.Enqueue(data);

            if (onGet != null)
            {
                onGet.Invoke(dataQueue);
            }
        }

        public void CallBack(IUIPanel panel, object data)
        {
            if (onCallBack != null) onCallBack.Invoke(panel, data);
        }

        public void Release()
        {
            if (onReleaseFromPool != null)
            {
                onReleaseFromPool(this);
            }
            if (onRelease != null)
            {
                onRelease(this);
            }
        }

        public void OnCreatePanel(IUIPanel panel)
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