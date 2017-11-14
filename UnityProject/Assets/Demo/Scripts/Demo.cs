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

public class Demo : MonoBehaviour {
    private void OnGUI()
    {
        if(GUILayout.Button("Open Panel01"))
        {
           var bridge = UIFacade.Instence.OpenPanel("Panel01");
            if(bridge)
            {
                bridge.callBack += (panel, data) =>
                {
                    Debug.Log(panel);
                };
            }
            else
            {
                Debug.Log("open err");
            }
         
        }
    }
}
