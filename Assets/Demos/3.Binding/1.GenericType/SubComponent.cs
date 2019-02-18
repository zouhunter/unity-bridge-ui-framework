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

public class SubComponent : BridgeUI.BridgeUIControl {
    public List<string> listTest { get; set; }
    public string[] stringTest { get; set; }
    public Dictionary<string, string> dicTest { get; set; }

    public List<List<string[]>> complexList { get; set; }

    protected override void OnInititalize()
    {
    }

    protected override void OnUnInitialize()
    {
    }
}
