using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Common;
using BridgeUI;

public class GraphPanelTest : MonoBehaviour {
    [SerializeField]
    private SelectorDataRoot selecorData;

    private void OnGUI()
    {
        if (GUILayout.Button("Open Graph 0"))
        {
            UIFacade.Instence.Open("GraphPanel", selecorData);
            //.RegistCallBack((p,x)=> selecorData = x as SelectorDataRoot);
        }
    }
}
