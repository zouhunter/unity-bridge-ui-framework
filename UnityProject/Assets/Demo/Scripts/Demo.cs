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

public class Demo : MonoBehaviour
{
    private const string pane01 = "Panel01";
    private UIFacade uiFacade;
    private void Awake()
    {
        uiFacade = UIFacade.Instence; 
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Open(1) " + pane01))
        {
            var handle = uiFacade.Open(pane01);
            handle.callBack += (panel, data) =>
            {
                Debug.Log(panel);
            };
        }
        if (GUILayout.Button("Open(2) " + pane01))
        {
            var handle = uiFacade.Open(pane01);
            handle.callBack += (panel, data) =>
            {
                Debug.Log(panel);
            };
        }
        if (GUILayout.Button("Close " + pane01))
        {
            uiFacade.Close(pane01);
        }
        if (GUILayout.Button("Hide " + pane01))
        {
            uiFacade.Hide(pane01);
        }
    }
}
