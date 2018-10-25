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
    //public ViewModel001 vmData;
    //private IUIHandle handle;
    //private void Awake()
    //{
    //    if (vmData == null)
    //        vmData = ScriptableObject.CreateInstance<ViewModel001>();
    //}

    //private void Update()
    //{
    //    vmData.fontSize = (int)Mathf.Lerp(10, 20, Time.time % 1);
    //}

    //private void OnGUI()
    //{
    //    if (GUILayout.Button("打开VMUsePanel"))
    //    {
    //        OpenSend(null);
    //    }

    //    if (GUILayout.Button("打开VMUsePanel -viewmodel001"))
    //    {
    //        OpenSend(vmData);
    //    }

    //    if (GUILayout.Button("打开VMUsePanel -runtime info"))
    //    {
    //        var hashTable = new Hashtable();
    //        hashTable.Add("info", "runtime info");
    //        OpenSend(hashTable);
    //    }

    //    if (GUILayout.Button("更新标题"))
    //    {
    //        vmData.title = "i`m vmData.title.Value";
    //    }
    //}

    //void OpenSend(object data)
    //{
    //    if(handle == null|| !handle.Active || handle.PanelName != PanelNames.VMUsePanel)
    //    {
    //        handle = UIFacade.Instence.Open(PanelNames.VMUsePanel); 
    //    }
    //    handle.Send(data);
    //}
}
