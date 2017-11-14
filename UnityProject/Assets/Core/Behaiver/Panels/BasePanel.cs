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
using System;

public class PanelBase :MonoBehaviour, IPanelBase
{
    public int InstenceID
    {
        get
        {
            return GetInstanceID();
        }
    }

    public string Name { get { return name; } }

    public IPanelGroup Group { get; set; }

    public void HandleCallBack(BridgeObj bridge, object data)
    {
        throw new NotImplementedException();
    }

    public void HandleData(object data)
    {
        throw new NotImplementedException();
    }
}
