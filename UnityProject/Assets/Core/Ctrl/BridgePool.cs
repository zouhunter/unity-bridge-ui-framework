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

public class BridgePool: IObjectPool<BridgeObj>
{
    private BridgeObj bridgePrefab;
    private ObjectPool<BridgeObj> innerPool;
    public BridgePool(BridgeObj bridgeObj)
    {
        this.bridgePrefab = bridgeObj;
        innerPool = new ObjectPool<BridgeObj>(1, CreateInstence);
    }

    public BridgeObj Allocate()
    {
        return innerPool.Allocate();
    }

    public BridgeObj CreateInstence()
    {
        return Object.Instantiate(bridgePrefab);
    }
}
