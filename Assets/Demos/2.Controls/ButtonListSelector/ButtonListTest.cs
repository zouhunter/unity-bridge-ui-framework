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
using System;

public class ButtonListTest : MonoBehaviour {
    public BridgeUI.Control.ButtonListSelector buttonList;
    public string[] options_A;
    public string[] options_B;

    string[][] options;
    int index;

    void Start()
    {
        options = new string[][] { options_A, options_B };
        buttonList.onSelectID.AddListener( OnSelectItem);
        buttonList.Initialize(null);
        buttonList.options = options[0];
    }

    private void OnGUI()
    {
       

        if (GUILayout.Button("初始化"))
        {
            buttonList.Initialize(null);
        }

        if (GUILayout.Button("去除初始化"))
        {
            buttonList.Recover();
        }
        if (GUILayout.Button("Switch"))
        {
            index++;
            if (index > 1)
            {
                index = 0;
            }
            buttonList.options = options[index];
        }
        if (GUILayout.Button("Clear"))
        {
            buttonList.options = null;
        }
    }

    private void OnSelectItem(int arg0)
    {
        Debug.Log("选中：" + options_A[arg0]);
    }
}
