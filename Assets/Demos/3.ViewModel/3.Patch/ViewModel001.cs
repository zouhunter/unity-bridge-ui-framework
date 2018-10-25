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
using BridgeUI.Binding;
using System;

public class ViewModel001 : VMUsePanel_ViewModel
{ 
    [SerializeField] private string _title;
    [SerializeField] private string _info;
    [SerializeField] private int _fontSize;

    private void OnEnable()
    {
        title = _title;
        info = _info;
        fontSize = _fontSize;
        onClick = OpenPanel01;
    }

    private void OpenPanel01(IBindingContext panel, Button sender)
    {
        Debug.Log("OpenPanel01");
        title = "panel:" + panel;
        info = "sender:" + sender;
    }
}
