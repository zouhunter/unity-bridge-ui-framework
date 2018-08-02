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
    IUIHandle handle;
    private void Awake()
    {
        LogSetting.objectPoolLog = true;
        infoDic["queue"] = false;
    }


    private void OnGUI()
    {
        if (GUILayout.Button("Open PopUpPanel 1000 time"))
        {
            StartCoroutine(OpenPopUnPanel1000());
        }

        if (GUILayout.Button("Stop Open"))
        {
            StopAllCoroutines();
        }

        if(GUILayout.Button("Send By Handle"))
        {
            if (handle != null)
            {
                foreach (var item in handle.Contexts)
                {
                    Debug.Log("context:" + item);
                }

                infoDic["title"] = "Handle Title";
                infoDic["info"] = "Handle Info";
                handle.Send(infoDic);
            }
            else
            {
                Debug.Log("handel have been Released!");
            }
        }
    }

    private IEnumerator OpenPopUnPanel1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            yield return new WaitForEndOfFrame();
            infoDic["title"] = string.Format("第{0}次打开", i);
            infoDic["info"] = "随机码：" + UnityEngine.Random.Range(0, 10000).ToString();
            handle = UIFacade.Instence.Open(PanelNames.PopUpPanel, infoDic);
            handle.RegistCallBack(OnCallBack);
            handle.RegistClose(OnClose);
            handle.RegistCreate(OnCreate);
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
