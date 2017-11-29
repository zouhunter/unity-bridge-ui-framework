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

public class Demo : MonoBehaviour
{
    private const string pane01 = "Panel01";
    private IUIFacade uiFacade;
    private void Awake()
    {
        uiFacade = UIFacade.Instence;
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Open:MainPanel"))
        {
            uiFacade.Open(PanelNames.MainPanel);
        }
        for (int i = 0; i < 2; i++)
        {
            if(GUILayout.Button("Open:Panel01  " + i))
            {
                OpenPanel01(i);
            }
        }
        if (GUILayout.Button("Close " + pane01))
        {
            uiFacade.Close(pane01);
        }
        if (GUILayout.Button("Hide " + pane01))
        {
            uiFacade.Hide(pane01);
        }
    }

    private void OpenPanel01(int index)
    {
        var handle = uiFacade.Open(pane01, index + "你好panel01");
        Debug.Log(index + "handle:" + handle);
        handle.onCallBack = (panel, data) =>
        {
            Debug.Log(index + "onCallBack" + panel + ":" + data);
        };
        handle.onCreate = (panel) =>
        {
            Debug.Log(index + "onCreate:" + panel);
        };
        handle.onClose = (panel) =>
        {
            Debug.Log(index + "onCloese:" + panel);
        };

        handle.Send(index + "你好panel01 _ send");
    }
}
