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
using BridgeUI.Binding;

public class ViewModel002 : BridgeUI.Binding.ViewModel
{
    public BridgeUI.Binding.PanelAction<UnityEngine.UI.Button> onClick
    {
        get
        {
            return GetValue<BridgeUI.Binding.PanelAction<UnityEngine.UI.Button>>("onClick");
        }
        set
        {
            SetValue<BridgeUI.Binding.PanelAction<UnityEngine.UI.Button>>("onClick", value);
        }
    }
    public System.String title
    {
        get
        {
            return GetValue<System.String>("title");
        }
        set
        {
            SetValue<System.String>("title", value);
        }
    }
    public System.Int32 fontSize
    {
        get
        {
            return GetValue<System.Int32>("fontSize");
        }
        set
        {
            SetValue<System.Int32>("fontSize", value);
        }
    }
    public System.String info
    {
        get
        {
            return GetValue<System.String>("info");
        }
        set
        {
            SetValue<System.String>("info", value);
        }
    }
  
    public ViewModel002()
    {
        onClick = OpenPanel02;
        title = "ViewModel02";
        info = "支持非ScriptObject类型的ViewModel";
    }

    private void OpenPanel02(IBindingContext panel, Button sender)
    {
        Debug.Log("OpenPanel02");
        title = "panel:" + panel;
        info = "sender:" + sender;
    }
}

public class VM_test : MonoBehaviour
{
    public ViewModel002 vmData;
    private IUIHandle handle;
    private void Awake()
    {
        if (vmData == null)
            vmData = new ViewModel002() ;
    }

    private void Update()
    {
        vmData.fontSize = (int)Mathf.Lerp(10, 20, Time.time % 1);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("打开VMUsePanel"))
        {
            OpenSend(null);
        }

        if (GUILayout.Button("打开VMUsePanel -viewmodel002"))
        {
            OpenSend(vmData);
        }

        if (GUILayout.Button("打开VMUsePanel -runtime info"))
        {
            var hashTable = new Hashtable();
            hashTable.Add("info", "runtime info");
            OpenSend(hashTable);
        }

        if (GUILayout.Button("更新标题"))
        {
            vmData.title = "i`m vmData.title.Value";
        }
    }

    void OpenSend(object data)
    {
        if (handle == null || !handle.Active || handle.PanelName != PanelNames.VMUsePanel)
        {
            handle = UIFacade.Instence.Open(PanelNames.VMUsePanel);
        }
        handle.Send(data);
    }
}
