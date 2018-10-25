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
using BridgeUI.Model;
namespace BridgeUI
{
    public class BridgePool
    {
        //private BridgeInfo bridgePrefab;
        private ObjectPool<Bridge> innerPool;
        public BridgePool(/*BridgeInfo bridgeObj*/)
        {
            //this.bridgePrefab = bridgeObj;
            innerPool = new ObjectPool<Bridge>(CreateInstence);
        }

        public Bridge CreateInstence()
        {
            var bridge = new Bridge( OnRelease);
            return bridge;
        }

        internal Bridge Allocate(BridgeInfo info,IUIPanel parentPanel = null)
        {
            var bridge = innerPool.Allocate();
            bridge.Reset(info,parentPanel);
            return bridge;
        }
        private void OnRelease(Bridge bridge)
        {
            innerPool.Release(bridge);
        }
    }
}