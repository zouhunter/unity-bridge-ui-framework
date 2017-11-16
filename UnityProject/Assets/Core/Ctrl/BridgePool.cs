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

public class BridgePool: IObjectPool<Bridge>
{
    private Bridge bridgePrefab;
    private ObjectPool<Bridge> innerPool;
    public BridgePool(Bridge bridgeObj)
    {
        this.bridgePrefab = bridgeObj;
        innerPool = new ObjectPool<Bridge>(1, CreateInstence);
    }

    public Bridge Allocate()
    {
        return innerPool.Allocate();
    }

    public Bridge CreateInstence()
    {
        var bridge = new Bridge();
        bridge.inNode = bridgePrefab.inNode;
        bridge.outNode = bridgePrefab.outNode;
        bridge.showModel = bridgePrefab.showModel;
        return bridge;
    }
}
