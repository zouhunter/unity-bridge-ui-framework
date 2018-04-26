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

public class CommonPanelTest : MonoBehaviour {
    private void OnGUI()
    {
        if(GUILayout.Button("PopUpPanel"))
        {
            UIFacade.Instence.Open(PanelNames.PopUpPanel, new string[] { "你好", "这是一个提示面板!" });
        }
    }
}
