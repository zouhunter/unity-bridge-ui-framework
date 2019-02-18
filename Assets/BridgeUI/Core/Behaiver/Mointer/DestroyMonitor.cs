using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BridgeUI
{
    /// <summary>
    /// 监听删除事件
    /// </summary>
    public class DestroyMonitor : MonoBehaviour
    {
        public UnityAction onDestroy { get; internal set; }

        protected virtual void OnDestroy()
        {
            if (onDestroy != null)
                onDestroy.Invoke();
        }
    }
}