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

public class VM_test : MonoBehaviour
{
    public ViewModel001 vmData;
    Dictionary<string, BridgeUI.Binding.IBindableProperty> vmDic;
    BridgeUI.Binding.B_String title = new BridgeUI.Binding.B_String("runtimme title");

    private void Awake()
    {
        if (vmData == null)
            vmData = ScriptableObject.CreateInstance<ViewModel001>();

        vmDic = new Dictionary<string, BridgeUI.Binding.IBindableProperty>();
        vmDic["title"] = title;
        vmDic["OpenPanel01"] = new BridgeUI.Binding.C_Button((panel, sender) =>
        {
            Debug.Log("OpenPanel01");
            title.Value = "panel:" + panel;
            vmDic["info"].ValueBoxed = "sender:" + sender;
        });
        vmDic["info"] = new BridgeUI.Binding.B_String("runtimme info");
    }

    private void Update()
    {
        vmData.fontSize = (int)Mathf.Lerp(10, 20, Time.time % 1);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("打开VMUsePanel"))
        {
            UIFacade.Instence.Open(PanelNames.VMUsePanel);
        }

        if (GUILayout.Button("打开VMUsePanel -viewmodel001"))
        {
            UIFacade.Instence.Open(PanelNames.VMUsePanel, vmData);
        }

        if (GUILayout.Button("打开VMUsePanel -viewmodel dic"))
        {
            UIFacade.Instence.Open(PanelNames.VMUsePanel, vmDic);
        }

        if (GUILayout.Button("更新标题"))
        {
            title.Value = "i`m title.Value ";
            vmData.title = "i`m vmData.title.Value";
        }
    }
}
