using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using System;

public partial class MyPanel
{
    protected override void Awake()
    {
        base.Awake();
        RegistEvents();
    }

    private void RegistEvents()
    {
        m_slider.onValueChanged.AddListener((value) =>
        {
            Debug.Log("slider: " + value);
        });
    }

    IEnumerator Start()
    {
        yield return null;
    }
}
