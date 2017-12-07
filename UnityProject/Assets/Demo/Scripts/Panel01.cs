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
using BridgeUI;

class Panel01 : SinglePanel, IPointerClickHandler
{
    [SerializeField]
    private Button m_Close;
    [SerializeField]
    private Button m_childPanel;

    [Charge(0)]
    private string testReceive { set { Debug.Log(value); } }
    protected override void Awake()
    {
        base.Awake();
        m_Close.onClick.AddListener(Close);
        m_childPanel.onClick.AddListener(OpenChildPanel);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        CallBack("panel01 发送的回调");
    }
    private void OpenChildPanel()
    {
        selfFacade.Open(PanelNames.Panel02, "你好panel02");
    }
}
