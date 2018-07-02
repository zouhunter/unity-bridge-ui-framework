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

public class toggleListSelectorTest : MonoBehaviour {
    public BridgeUI.Control.ListSelector toggleList;
    public string[] options;

    public void Start()
    {
        toggleList.onSelectID += onSelectOne;
        toggleList.onSelectIDs += OnSelectMany;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            options = new string[3];
            options[0] = "A";
            options[1] = "B";
            options[2] = "C";
            toggleList.options = options;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            options = new string[3];
            options[0] = "B";
            options[1] = "C";
            options[2] = "D";
            toggleList.options = options;
        }
    }
    void OnGUI()
    {
        GUILayout.Label("输入A 或者 B");
    }
    private void OnSelectMany(int[] arg0)
    {
        Debug.Log("选择了：");
        foreach (var item in arg0)
        {
            Debug.Log(item);
        }
    }

    private void onSelectOne(int arg0)
    {
        Debug.Log("选择了：" + arg0);
    }
}


