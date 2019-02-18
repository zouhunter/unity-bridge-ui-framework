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
using BridgeUI;
using System;

/// <summary>
/// 测试缓存池的工作状况
/// </summary>
public class objectPoolTest : MonoBehaviour {
    Hashtable infoDic = new Hashtable();
    private PanelVisitor handleA;
    private PanelVisitor handleB;

    private void Awake()
    {
        LogSetting.objectPoolLog = true;
        infoDic["queue"] = false;
        handleA = new PanelVisitor(infoDic);
        handleB = new PanelVisitor();
        handleA.onCreate = (x) => { Debug.Log("handle a callBack: oncreate"); };
        handleA.onClose = (x) => { Debug.Log("handle a callBack: onclose"); };
        handleA.onCallBack = (x,y) => { Debug.Log("handle a callBack: oncallback"); };
        UIFacade.Instence.RegistClose(OnClose);
        UIFacade.Instence.RegistCreate(OnCreate);
    }


    private void OnGUI()
    {
        if (GUILayout.Button("Open PopUpPanel Once time (HandleA)"))
        {
            infoDic["title"] = "随机标题：" + UnityEngine.Random.Range(0, 10000).ToString();
            infoDic["info"] = "随机码：" + UnityEngine.Random.Range(0, 10000).ToString();
            UIFacade.Instence.Open(PanelNames.PopupPanel, handleA);
        }

        if (GUILayout.Button("Open PopUpPanel 1000 time (HandleB)"))
        {
            StartCoroutine(OpenPopUnPanel1000HandleB());
        }

        if (GUILayout.Button("Stop Open"))
        {
            StopAllCoroutines();
        }

        if(GUILayout.Button("Send By handleB"))
        {
            if (handleB != null)
            {
                infoDic["title"] = "Handle Title";
                infoDic["info"] = "Handle Info";
                handleB.Send(infoDic);
            }
            else
            {
                Debug.Log("handel have been Released!");
            }
        }
    }

    private IEnumerator OpenPopUnPanel1000HandleB()
    {
        handleB = new PanelVisitor(infoDic);
        handleB.onCallBack = OnCallBack;
        for (int i = 0; i < 1000; i++)
        {
            yield return new WaitForEndOfFrame();
            infoDic["title"] = string.Format("第{0}次打开", i);
            infoDic["info"] = "随机码：" + UnityEngine.Random.Range(0, 10000).ToString();
            UIFacade.Instence.Open(PanelNames.PopupPanel, handleB);
        }
    }

    private void OnCreate(IUIPanel arg0)
    {
        Debug.Log("OnCreate:" + arg0);
    }

    private void OnClose(IUIPanel arg0)
    {
        Debug.Log("OnClose:" + arg0);
    }
    private void OnCallBack(IUIPanel arg0, object arg1)
    {
        Debug.Log("OnCallBack:" + arg0);
    }
}
