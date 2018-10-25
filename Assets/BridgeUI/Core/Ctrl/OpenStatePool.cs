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
    public class OpenState
    {
        public string panelName;
        public object data;
        public IUIPanel parent;
        public int index;
    }

    public class OpenStatePool
    {
        private ObjectPool<OpenState> innerPool;

        public OpenStatePool()
        {
            innerPool = new ObjectPool<OpenState>(()=> new OpenState());
        }

        public OpenState Allocate(IUIPanel parent, string panelName, object data = null,int index = -1)
        {
            var handle = innerPool.Allocate();
            handle.parent = parent;
            handle.panelName = panelName;
            handle.data = data;
            handle.index = index;
            return handle;
        }

        public void Release(OpenState handle)
        {
            innerPool.Release(handle);
        }
    }
}