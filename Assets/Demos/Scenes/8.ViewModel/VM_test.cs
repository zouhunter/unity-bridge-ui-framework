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
    ViewModel001 vmData = new ViewModel001();
    Dictionary<string, BridgeUI.Binding.IBindableProperty> vmData1;
    BridgeUI.Binding.B_String title = new BridgeUI.Binding.B_String("runtimme title");

    private void Awake()
    {
        vmData1 = new Dictionary<string, BridgeUI.Binding.IBindableProperty>();
        vmData1["title"] = title;
        vmData1["OpenPanel01"] = new BridgeUI.Binding.C_Button((panel, sender) =>
        {
            Debug.Log("OpenPanel01");
            title.Value = "panel:" + panel;
            vmData1["info"].ValueBoxed = "sender:" + sender;
        });
        vmData1["info"] = new BridgeUI.Binding.B_String("runtimme info");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("打开主界面-viewmodel001"))
        {
            UIFacade.Instence.Open(PanelNames.MainPanel, vmData);
        }

        if (GUILayout.Button("打开主界面-viewmodel dic"))
        {
            UIFacade.Instence.Open(PanelNames.MainPanel, vmData1);
        }

        if (GUILayout.Button("更新标题"))
        {
            title.Value = "i`m title.Value ";
            vmData.title = "i`m vmData.title.Value";
        }
    }
}
