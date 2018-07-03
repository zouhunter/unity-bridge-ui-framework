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

public class customComponent : MonoBehaviour {
    public customEvent01 customEvent = new customEvent01();
    [SerializeField]
    private Button m_trigger;

    private void Awake()
    {
        m_trigger.onClick.AddListener(Trigger);
    }

    private void Trigger()
    {
        customEvent.Invoke("演示如果使用自定义事件", 200);
    }
}
