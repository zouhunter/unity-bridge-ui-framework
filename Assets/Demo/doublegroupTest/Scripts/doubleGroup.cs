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
using BridgeUI.Model;
using BridgeUI;
using System;

public class doubleGroup : MonoBehaviour
{
    private const string pane01 = "Panel01";
    private IUIFacade uiFacade;
    private MainPanelViewModel mainViewModel;
    private void Awake()
    {
        uiFacade = UIFacade.Instence;
        mainViewModel = new MainPanelViewModel();
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Open:MainPanel"))
        {
            var dic = new Dictionary<string, object>();
            dic["title"] = "我是主面板";
            uiFacade.Open(PanelNames.MainPanel, dic);
        }
        if (GUILayout.Button("Open:MainPanel with viewModel"))
        {
            uiFacade.Open(PanelNames.MainPanel, mainViewModel);
        }
        for (int i = 0; i < 2; i++)
        {
            if (GUILayout.Button("Open:Panel01  " + i))
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

    private void Update()
    {
        if(Input.GetMouseButtonDown(1)){
            mainViewModel.switcher.Value = !mainViewModel.switcher.Value;
        }
        mainViewModel.title.Value = UnityEngine.Random.Range(0,100).ToString();
    }

    private void OpenPanel01(int index)
    {
        var dic = new Hashtable();
        dic[0] = "我是panel01";
        var handle = uiFacade.Open(pane01, index + "你好panel01");
        Debug.Log(index + "handle:" + handle);
        handle.RegistCallBack((panel, data) =>
        {
            Debug.Log(index + "onCallBack" + panel + ":" + data);
        });
        handle.RegistCreate((panel) =>
        {
            Debug.Log(index + "onCreate:" + panel);
        });
        handle.RegistClose((panel) =>
        {
            Debug.Log(index + "onCloese:" + panel);
        });

        handle.Send(dic);
    }
}
