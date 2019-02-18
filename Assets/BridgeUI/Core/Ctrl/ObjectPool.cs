using System;
using UnityEngine;
using System.Collections.Generic;

namespace BridgeUI
{
    public class ObjectPool<T>
    {
#if BridgeUI_Log
        bool log { get { return LogSetting.objectPoolLog; } }
#endif
        public UnityEngine.Events.UnityAction<T> onCreate { get; set; }
        public UnityEngine.Events.UnityAction<T> onRelease { get; set; }
        protected virtual Func<T> createFunc { get; set; }
        protected Stack<T> stack;
        public ObjectPool()
        {
            this.createFunc = new Func<T>(() => Activator.CreateInstance<T>());
            this.stack = new Stack<T>();
        }

        public ObjectPool(Func<T> createFunc)
        {
            this.createFunc = createFunc;
            this.stack = new Stack<T>();
        }

        internal T Allocate()
        {
            if (stack.Count == 0)
            {
#if BridgeUI_Log
                if (log) Debug.Log("create new " + typeof(T).FullName);
#endif
                var instence = createFunc();
                if (onCreate != null)
                {
                    onCreate.Invoke(instence);
                }
                return instence;
            }
            else
            {
                return stack.Pop();
            }
        }
        internal void Release(T instence)
        {
#if BridgeUI_Log
            if (log) Debug.Log("release: " + instence);
#endif
            stack.Push(instence);
            if (onRelease != null)
                onRelease.Invoke(instence);
        }
    }
}