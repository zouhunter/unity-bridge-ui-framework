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

public class UIHandlePool : IObjectPool<UIHandle>
{
    private ObjectPool<UIHandle> innerPool;
    public UIHandlePool()
    {
        innerPool = new ObjectPool<UIHandle>(1,CreateInstence);
    }

    public UIHandle Allocate()
    {
        return innerPool.Allocate();
    }

    private UIHandle CreateInstence()
    {
        return new UIHandle();
    }
}
