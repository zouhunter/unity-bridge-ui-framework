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
using BridgeUI.Control;

public class radiamenuTest : MonoBehaviour {

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            UIFacade.Instence.Open(PanelNames.RadiaMenuPanel,new UnityAction<RadialMenu>( (x) => {
                var menu = x as RadialMenu;
                menu.m_MenuName = "测试menu";
                menu.AddSlider("slider", 0, 100, 20, (sliderValue) => { Debug.Log("SliderValue:" + sliderValue); });
                menu.AddList("list",new string[] { "A","B","C","D"}, (listValue) => { Debug.Log("ListValue:" + listValue); });
            }));
        }
    }
}
