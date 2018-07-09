using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Control;
using System;

public class ToolWorpBehaiver : ToolItemBehaiver
{
    //对应的目标
    public GameObject placeItem { get; set; }
 
    public override bool TryUse(UnityAction onUse)
    {
        return false;
    }

    public override void SetPosition(Vector3 pos)
    {
        base.SetPosition(pos);
        if(placeItem != null)
        {
            placeItem.transform.position = pos;
            transform.position = Vector3.one * 1000;
        }
        else
        {
            transform.position = pos;
        }
    }

}
