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
        private BridgeInfo bridgePrefab;
        private ObjectPool<Bridge> innerPool;
        public BridgePool(BridgeInfo bridgeObj)
        {
            this.bridgePrefab = bridgeObj;
            innerPool = new ObjectPool<Bridge>(1, CreateInstence);
        }

        public Bridge CreateInstence()
        {
            var bridge = new Bridge(bridgePrefab);
            return bridge;
        }

        internal Bridge Allocate(IPanelBaseInternal parentPanel = null)
        {
            var bridge = innerPool.Allocate();
            bridge.Reset(parentPanel);
            return bridge;
        }
    }
}