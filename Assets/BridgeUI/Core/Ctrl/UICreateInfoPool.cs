using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BridgeUI
{
    public class UICreateInfoPool
    {
        private ObjectPool<UICreateInfo> innerPool;
        public UICreateInfoPool()
        {
            innerPool = new ObjectPool<UICreateInfo>(CreateInstence);
        }

        public UICreateInfo CreateInstence()
        {
            var bridge = new UICreateInfo();
            return bridge;
        }

        internal UICreateInfo Allocate(UIInfoBase info, UnityAction<GameObject> onCreate)
        {
            var uicreateInfo = innerPool.Allocate();
            uicreateInfo.uiInfo = info;
            uicreateInfo.onCreate = onCreate;
            return uicreateInfo;
        }
        public void Release(UICreateInfo uiCreateInfo)
        {
            innerPool.Release(uiCreateInfo);
        }
    }
    public class UICreateInfo
    {
        public UIInfoBase uiInfo;
        public UnityAction<GameObject> onCreate;
    }
}