using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using BridgeUI;

public class RenderTestPanel : MonoBehaviour, ICustomUI
{
    [SerializeField]
    private Button m_close;
    [SerializeField]
    private Button m_body;
    private Button m_body0;

    private IUIPanel view;

    public Transform Content
    {
        get
        {
            return null;
        }
    }

    public void Initialize(IUIPanel view)
    {
        this.view = view;
        m_close.onClick.AddListener(this.view.Close);
        m_body.onClick.AddListener(OpenPopPanel);

        Debug.Log("Initialize" + this);
    }

    public void Recover()
    {
        m_close.onClick.RemoveListener(this.view.Close);
        m_body.onClick.RemoveListener(OpenPopPanel);

        Debug.Log("Recover" + this);
    }

    private void OpenPopPanel()
    {
        if(view!=null)
        {
            view.Open("PopupPanel", new string[] { "测试", "看关闭之后，刚才那个面板能打开不！" });
        }
    }

}
