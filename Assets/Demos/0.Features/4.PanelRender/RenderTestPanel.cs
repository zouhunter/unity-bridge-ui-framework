using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using BridgeUI;

[RequireComponent(typeof(BridgeUI.PanelCore))]
public class RenderTestPanel : MonoBehaviour {
    [SerializeField]
    private Button m_close;
    [SerializeField]
    private Button m_body;
    private Button m_body0;

    private BridgeUI.PanelCore panelRender;

    private void Awake()
    {
        panelRender = GetComponent<BridgeUI.PanelCore>();
        m_close.onClick.AddListener(panelRender.Close);
        m_body.onClick.AddListener(OpenPopPanel);
    }

    private void OpenPopPanel()
    {
        panelRender.Open("PopupPanel",new string[] {"测试","看关闭之后，刚才那个面板能打开不！" });
    }
}
