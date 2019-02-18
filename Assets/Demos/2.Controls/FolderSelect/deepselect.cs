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
using BridgeUI.Control;
using BridgeUI.Control.Tree;
using System;

public class deepselect : MonoBehaviour {
    public FolderSelector selector;
    public TreeNode1 rootNode;
    void Start()
    {
        selector.Initialize(null);
        selector.onSelect = OnSelect;
        selector.CreateTree(rootNode);
    }

    private void OnSelect(string[] arg0)
    {
        if (arg0 == null) return;
        Debug.Log(string.Join("/", arg0));
    }

    void OnGUI()
    {
        if (GUILayout.Button("0,0"))
        {
            selector.SetSelect(0, 0);
        }
        if (GUILayout.Button("A/a0"))
        {
            selector.SetSelect("A", "a0");
        }
        if (GUILayout.Button("Auto First"))
        {
            selector.AutoSelectFirst();
        }
    }
}
