using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using System;
namespace BridgeUI
{
    public class UIHandlePool
    {
        private List<UIHandle> activedHandles = new List<UIHandle>();
        private ObjectPool<UIHandle> innerPool;

        public UIHandlePool()
        {
            innerPool = new ObjectPool<UIHandle>(CreateInstence);
        }

        public UIHandle Allocate(string panelName)
        {
            var handle = activedHandles.Find(x => x.PanelName == panelName);
            if(handle != null)
            {
                return handle;
            }
            else
            {
                handle = innerPool.Allocate();
                activedHandles.Add(handle);
                handle.Reset(panelName, OnRelease);
            }
            return handle;
        }

        private UIHandle CreateInstence()
        {
            return new UIHandle();
        }

        private void OnRelease(UIHandle handle)
        {
            activedHandles.Remove(handle);
            innerPool.Release(handle);
        }
    }
}