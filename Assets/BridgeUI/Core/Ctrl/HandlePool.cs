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
        private Dictionary<string, UIHandle> activedHandles;
        private ObjectPool<UIHandle> innerPool;

        public UIHandlePool()
        {
            activedHandles = new Dictionary<string, UIHandle>();
            innerPool = new ObjectPool<UIHandle>();
        }

        public UIHandle Allocate(string panelName)
        {
            UIHandle uiHandle;
            if (activedHandles.TryGetValue(panelName,out uiHandle))
            {
                return uiHandle;
            }
            else
            {
                uiHandle = innerPool.Allocate();
                activedHandles.Add(panelName, uiHandle);
                uiHandle.Reset(panelName, OnRelease);
                return uiHandle;
            }
        }

        private UIHandle CreateInstence()
        {
            return new UIHandle();
        }

        private void OnRelease(UIHandle handle)
        {
            activedHandles.Remove(handle.PanelName);
            innerPool.Release(handle);
        }
    }
}