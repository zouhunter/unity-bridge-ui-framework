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

public class ButtonListTest : MonoBehaviour {
    public BridgeUI.Control.ButtonListSelector buttonList;
    public string[] options;

    void Start()
    {
        buttonList.onSelectID += OnSelectItem;
        buttonList.options = options;
    }

    private void OnSelectItem(int arg0)
    {
        Debug.Log("选中：" + options[arg0]);
    }
}
