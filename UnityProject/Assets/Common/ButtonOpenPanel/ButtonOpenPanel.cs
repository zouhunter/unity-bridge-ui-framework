using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using System.Reflection;
using System.Linq;
using System;

/// <summary>
///利用按扭打开其他面板
/// </summary>

public class ButtonOpenPanel : SinglePanel
{
    [System.Serializable]
    public class Holder
    {
        public string panelName;
        public Button button;
    }
    public List<Holder> holders = new List<Holder>();

    protected override void Awake()
    {
        base.Awake();
        foreach (var item in holders)
        {
            if (item.button != null && !string.IsNullOrEmpty(item.panelName))
            {
                item.button.onClick.AddListener(()=> { this.Open (item.panelName); });
            }
        }
    }
}
